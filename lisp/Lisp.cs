using System;
using SysArray = System.Array;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Shelisp {

	public class L {
		static L ()
		{
			root_environment = Qnil;
			RegisterGlobalBuiltins (typeof (L).Assembly);

			// dump out our environment
			if (root_environment != Qnil) {
				foreach (var binding in (List)root_environment) {
					if (NILP(binding))
						break;

					Debug.Print ("{0}", CAR(binding));
				}
			}

			Qcdr = intern ("cdr");
			Qcar = intern ("car");
			Qlambda = intern ("lambda");
			Qmacro = intern ("macro");
			Qquote = intern ("quote");
			Qt = intern ("t");
			((Symbol)Qt).value = Qt;
			Qnil = intern ("nil");
			((Symbol)Qnil).value = Qnil;
			Qunbound = intern ("unbound");
		}

		public L ()
		{
			Environment = root_environment;
			features = new List<string>();
		}

		public static void RegisterGlobalBuiltins (Assembly assembly)
		{
			foreach (var t in assembly.GetTypes())
				RegisterGlobalBuiltins (t);
		}

		public static void RegisterGlobalBuiltins (Type t)
		{
			foreach (var method in t.GetMethods()) {
				var builtin_attrs = method.GetCustomAttributes (typeof (LispBuiltinAttribute), true);
				if (builtin_attrs == null || builtin_attrs.Length == 0)
					continue;
				foreach (var ba in builtin_attrs) {
					LispBuiltinAttribute builtin_attr = (LispBuiltinAttribute)ba;

					Debug.Print ("found [LispBuiltin({0})] on '{1}.{2}'", builtin_attr.Name, method.DeclaringType, method.Name);

					string doc_string = builtin_attr.DocString == null ? "" : builtin_attr.DocString;

					Symbol s = L.intern (builtin_attr.Name);
					s.function = L.DEFUN_internal (builtin_attr.Name, doc_string, builtin_attr.MinArgs, builtin_attr.Unevalled, method);

					root_environment = new List (new List (s, s), root_environment);
				}
			}
		}

		List<string> features;

		public bool IsFeatureLoaded (string feature_name)
		{
			return features.Contains (feature_name);
		}

		public void AddFeature (string feature_name)
		{
			if (!IsFeatureLoaded (feature_name))
				features.Add (feature_name);
		}

		// our current executing environment
		public Shelisp.Object Environment { get; set; }

		// the environment that contains all the builtin definitions
		static Shelisp.Object root_environment;

		public static Shelisp.Object Qcdr;
		public static Shelisp.Object Qcar;
		public static Shelisp.Object Qt;
		public static Shelisp.Object Qnil;
		public static Shelisp.Object Qlambda;
		public static Shelisp.Object Qmacro;
		public static Shelisp.Object Qunbound;
		public static Shelisp.Object Qquote;

		public static Match current_match;

		public static Dictionary<string,Shelisp.Symbol> symbols = new Dictionary<string,Shelisp.Symbol>();

		public static Shelisp.Symbol intern (string str)
		{
			Shelisp.Symbol sym;
			if (!symbols.TryGetValue(str, out sym)) {
				sym = new Shelisp.Symbol (str);
				symbols[str] = sym;
			}
			return sym;
		}

		public static Shelisp.Object string_array_to_list (string[] arr)
		{
			Object cons = Qnil;
			for (int i = arr.Length - 1; i >= 0; i --)
				cons = new List (new Shelisp.String (arr[i]), cons);
			return cons;
		}

		public static Shelisp.Object make_list (params Shelisp.Object[] arr)
		{
			Object cons = Qnil;
			for (int i = arr.Length - 1; i >= 0; i --)
				cons = new List (arr[i], cons);
			return cons;
		}

		private static Shelisp.Subr DEFUN_internal (string lisp_name, string doc, int min_args, bool unevalled, MethodInfo meth, object target = null)
		{
			Subr s = new Subr (lisp_name, doc, 0, unevalled, meth);
			return s;
		}

		private static Shelisp.Subr DEFUN_internal (string lisp_name, string doc, int min_args, Delegate d)
		{
			return DEFUN_internal (lisp_name, doc, 0, false, d.Method, d.Target);
		}

		public static Shelisp.Subr DEFUN (string lisp_name, string doc, Action func)
		{
			return DEFUN_internal (lisp_name, doc, 0, func);
		}

		public static Shelisp.Subr DEFUN<T> (string lisp_name, string doc, int min_args, Func<T> func)
		{
			return DEFUN_internal (lisp_name, doc, min_args, func);
		}

		public static Shelisp.Subr DEFUN<T1,T2> (string lisp_name, string doc, int min_args, Func<T1,T2> func)
		{
			return DEFUN_internal (lisp_name, doc, min_args, func);
		}

		public static Shelisp.Subr DEFUN<T1,T2,T3> (string lisp_name, string doc, int min_args, Func<T1,T2,T3> func)
		{
			return DEFUN_internal (lisp_name, doc, min_args, func);
		}

		public static Shelisp.Subr DEFUN<T1,T2,T3,T4> (string lisp_name, string doc, int min_args, Func<T1,T2,T3,T4> func)
		{
			return DEFUN_internal (lisp_name, doc, min_args, func);
		}

		public static Shelisp.Object CAR (Shelisp.Object cons)
		{
			return ((List)cons).car;
		}

		public static Shelisp.Object CDR (Shelisp.Object cons)
		{
			return NILP(cons) ? Qnil : ((List)cons).cdr;
		}

		public static Shelisp.List CONS (Shelisp.Object cons)
		{
			return ((List)cons);
		}

		public static bool NILP (Shelisp.Object o)
		{
			return o == Qnil;
		}

		public static bool LISTP (Shelisp.Object o)
		{
			return o is List || o == Qnil;
		}

		public static bool CONSP (Shelisp.Object o)
		{
			return o is List;
		}

		// string foo
		public static int SCHARS (Shelisp.Object o)
		{
			return (o as String).native_string.Length;
		}

		public static char SREF (Shelisp.Object o, int c)
		{
			return (o as String).native_string[c];
		}

		[LispBuiltin ("function", MinArgs = 1, Unevalled = true)]
		public static Shelisp.Object Ffunction (L l, Shelisp.Object func)
		{
			return func;
		}

		[LispBuiltin ("interactive", MinArgs = 0)]
		public static Shelisp.Object Finteractive (L l, params Shelisp.Object[] args)
		{
			return L.Qnil;
		}

		[LispBuiltin ("error", MinArgs = 1)]
		public static Shelisp.Object Ferror (L l, Shelisp.Object format, params Shelisp.Object[] args)
		{
			// we need a C-styled sprintf/string.Format here
			throw new Exception ((string)(Shelisp.String)format);
		}

		[LispBuiltin ("add-hook", MinArgs = 2)]
		public static Shelisp.Object Fadd_hook (L l, Shelisp.Object hook, Shelisp.Object function, Shelisp.Object append, Shelisp.Object local)
		{
			return L.Qnil;
		}

		[LispBuiltin ("indirect-function", MinArgs = 1)]
		public static Shelisp.Object Findirect_function (L l, Shelisp.Object symorfunction)
		{
			Shelisp.Object sym = symorfunction;

			while (true) {
				if (sym is Symbol) {
					sym = ((Symbol)sym).function;
					continue;
				}
				else
					return sym;
			}
		}

		[LispBuiltin ("defun", MinArgs = 3, Unevalled = true, DocString = 
@"Define NAME as a function.
The definition is (lambda ARGLIST [DOCSTRING] BODY...).
See also the function `interactive'.
usage: (defun NAME ARGLIST [DOCSTRING] BODY...)")]
		public static Shelisp.Object Fdefun (L l, Shelisp.Object sym, Shelisp.Object arglist, params Shelisp.Object[] body_forms)
		{
			Console.WriteLine ("Fdefun ({0})", sym);

			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);

			var doc_string = "";

			if (body_forms.Length > 0) {
				if (body_forms[0] is Shelisp.String) {
					doc_string = (Shelisp.String)body_forms[0];
					Shelisp.Object[] new_body_forms = new Shelisp.Object[body_forms.Length - 1];
					SysArray.Copy (body_forms, 1, new_body_forms, 0, new_body_forms.Length);
					body_forms = new_body_forms;
				}
			}
			if (body_forms.Length == 0)
				throw new Exception ("0 length body forms");

			Shelisp.Object defn = List.Fnconc (l, L.make_list (L.Qlambda, arglist), new List (body_forms));

			Symbol s = (Symbol)sym;
			Symbol.Ffset (l, s, defn);
			// XXX more here I'm sure... like what do we do with the doc string?
			l.Environment = new List (new List(s, s), l.Environment);

			Console.WriteLine ("returning");
			return sym;
		}

		[LispBuiltin ("defalias", MinArgs = 2)]
		public static Shelisp.Object Fdefalias (L l, Shelisp.Object name, Shelisp.Object defn, Shelisp.Object doc_string)
		{
			if (!(name is Symbol))
				throw new WrongTypeArgumentException ("symbolp", name);

			Shelisp.Object fun;

			if (defn is Symbol)
				fun = ((Symbol)defn).function;
			else
				fun = defn;

			Symbol s = (Symbol)name;
			Symbol.Ffset (l, s, defn);

			// XXX more here I'm sure... like what do we do with the doc string?
			l.Environment = new List (new List(s, s), l.Environment);

			return fun;
		}

		[LispBuiltin ("defconst", MinArgs = 2, Unevalled = true)]
		public static Shelisp.Object Fdefconst (L l, Shelisp.Object sym, Shelisp.Object value, Shelisp.Object doc_string)
		{
			Console.WriteLine ("defconst {0} {1}", sym, value);
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);
			((Symbol)sym).value = value.Eval (l);
			return sym;
		}

		[LispBuiltin ("defmacro", MinArgs = 3, Unevalled = true)]
		public static Shelisp.Object Fdefmacro (L l, Shelisp.Object sym, Shelisp.Object arglist, params Shelisp.Object[] body_forms)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);

			var doc_string = "";

			if (body_forms.Length > 0) {
				if (body_forms[0] is Shelisp.String) {
					doc_string = (Shelisp.String)body_forms[0];
					Shelisp.Object[] new_body_forms = new Shelisp.Object[body_forms.Length - 1];
					SysArray.Copy (body_forms, 1, new_body_forms, 0, new_body_forms.Length);
					body_forms = new_body_forms;
				}
			}
			Shelisp.Object defn = List.Fnconc (l, L.make_list (L.Qmacro, L.Qlambda, arglist), body_forms.Length == 0 ? L.Qnil : new List (body_forms));

			Console.WriteLine ("defmacro {0} {1}", sym, defn);

			Symbol s = (Symbol)sym;
			Symbol.Ffset (l, s, defn);
			// XXX more here I'm sure... like what do we do with the doc string?
			l.Environment = new List (new List(s, s), l.Environment);

			return sym;
		}

		[LispBuiltin ("defvar", MinArgs = 1, Unevalled = true)]
		public static Shelisp.Object Fdefvar (L l, Shelisp.Object sym, Shelisp.Object value, Shelisp.Object docstring)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);
			((Symbol)sym).value = value;
			return sym;
		}

		[LispBuiltin ("defvaralias", MinArgs = 2)]
		public static Shelisp.Object Fdefvaralias (L l, Shelisp.Object name, Shelisp.Object base_variable, Shelisp.Object doc_string)
		{
			if (!(name is Symbol))
				throw new WrongTypeArgumentException ("symbolp", name);

			Shelisp.Object value;

			if (base_variable is Symbol)
				value = ((Symbol)base_variable).value;
			else
				value = base_variable;

			Symbol s = (Symbol)name;
			L.Fsetq (l, s, value);

			// XXX more here I'm sure... like what do we do with the doc string?
			l.Environment = new List (new List(s, s), l.Environment);

			return base_variable;
		}

		[LispBuiltin ("setq", MinArgs = 2, Unevalled = true)]
		public static Shelisp.Object Fsetq (L l, Shelisp.Object sym, Shelisp.Object val)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);

			var evalled = val.Eval(l);
			l.Environment = new List (new List(sym, evalled), l.Environment);
			return evalled;
		}

		[LispBuiltin ("set", MinArgs = 2)]
		public static Shelisp.Object Fset (L l, Shelisp.Object sym, Shelisp.Object val)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);

			l.Environment = new List (new List(sym, val), l.Environment);
			return val;
		}

		[LispBuiltin ("let", MinArgs = 2, Unevalled = true)]
		public static Shelisp.Object Flet (L l, Shelisp.Object bindings, params Shelisp.Object[] forms)
		{
			Console.WriteLine ("Flet bindings = {0}", bindings);
			foreach (var form in forms)
				Console.WriteLine (" + form = {0}", form);
			return L.Qnil;
		}

		[LispBuiltin ("current-time")]
		public static Shelisp.Object Fcurrent_time (L l)
		{
			DateTime unixRef = new DateTime(1970, 1, 1, 0, 0, 0);

			long ticks_unix = (DateTime.Now.Ticks - unixRef.Ticks);

			int high, low, microseconds;
			int seconds;

			seconds = (int)(ticks_unix / TimeSpan.TicksPerSecond);
			microseconds = (int)(ticks_unix % TimeSpan.TicksPerSecond);

			high = (int)(seconds / (2<<16));
			low = (int)(seconds % (2<<16));

			return L.make_list (high, low, microseconds);
		}

		[LispBuiltin ("system-name")]
		public static Shelisp.Object Fsystem_name (L l)
		{
			// XXX
			return (Shelisp.String)"localhost";
		}
	}
}

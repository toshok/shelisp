using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lisp {

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
			Qquote = intern ("quote");
			Qt = intern ("t");
			Qnil = intern ("nil");
			Qunbound = intern ("unbound");
		}

		public L ()
		{
			this.Environment = root_environment;
		}

		public static void RegisterGlobalBuiltins (Assembly assembly)
		{
			foreach (var t in assembly.GetTypes())
				RegisterGlobalBuiltins (t);
		}

		public static void RegisterGlobalBuiltins (Type t)
		{
			foreach (var method in t.GetMethods()) {
				var builtin_attr = (LispBuiltinAttribute)Attribute.GetCustomAttribute (method, typeof (LispBuiltinAttribute), true);
				if (builtin_attr == null)
					continue;
				Debug.Print ("found [LispBuiltin({0})] on '{0}.{1}'", builtin_attr.Name, method.DeclaringType, method.Name);

				string doc_string = builtin_attr.DocString == null ? "" : builtin_attr.DocString;

				Symbol s = L.intern (builtin_attr.Name);
				s.function = L.DEFUN_internal (builtin_attr.Name, doc_string, builtin_attr.MinArgs, builtin_attr.Unevalled, method);

				root_environment = new List (new List (s, s), root_environment);
			}
		}

		// our current executing environment
		public Lisp.Object Environment { get; set; }

		// the environment that contains all the builtin definitions
		static Lisp.Object root_environment;

		public static Lisp.Object Qcdr;
		public static Lisp.Object Qcar;
		public static Lisp.Object Qt;
		public static Lisp.Object Qnil;
		public static Lisp.Object Qlambda;
		public static Lisp.Object Qunbound;
		public static Lisp.Object Qquote;

		public static Dictionary<string,Lisp.Symbol> symbols = new Dictionary<string,Lisp.Symbol>();

		public static Lisp.Symbol intern (string str)
		{
			Lisp.Symbol sym;
			if (!symbols.TryGetValue(str, out sym)) {
				sym = new Lisp.Symbol (str);
				symbols[str] = sym;
			}
			return sym;
		}

		public static Lisp.List make_list (params Lisp.Object[] arr)
		{
			Object cons = Qnil;
			for (int i = arr.Length - 1; i >= 0; i --)
				cons = new List (arr[i], cons);
			return CONS(cons);
		}

		private static Lisp.Subr DEFUN_internal (string lisp_name, string doc, int min_args, bool unevalled, MethodInfo meth, object target = null)
		{
			Subr s = new Subr (lisp_name, doc, 0, unevalled, meth);
			return s;
		}

		private static Lisp.Subr DEFUN_internal (string lisp_name, string doc, int min_args, Delegate d)
		{
			return DEFUN_internal (lisp_name, doc, 0, false, d.Method, d.Target);
		}

		public static Lisp.Subr DEFUN (string lisp_name, string doc, Action func)
		{
			return DEFUN_internal (lisp_name, doc, 0, func);
		}

		public static Lisp.Subr DEFUN<T> (string lisp_name, string doc, int min_args, Func<T> func)
		{
			return DEFUN_internal (lisp_name, doc, min_args, func);
		}

		public static Lisp.Subr DEFUN<T1,T2> (string lisp_name, string doc, int min_args, Func<T1,T2> func)
		{
			return DEFUN_internal (lisp_name, doc, min_args, func);
		}

		public static Lisp.Subr DEFUN<T1,T2,T3> (string lisp_name, string doc, int min_args, Func<T1,T2,T3> func)
		{
			return DEFUN_internal (lisp_name, doc, min_args, func);
		}

		public static Lisp.Subr DEFUN<T1,T2,T3,T4> (string lisp_name, string doc, int min_args, Func<T1,T2,T3,T4> func)
		{
			return DEFUN_internal (lisp_name, doc, min_args, func);
		}

		public static Lisp.Object CAR (Lisp.Object cons)
		{
			return ((List)cons).car;
		}

		public static Lisp.Object CDR (Lisp.Object cons)
		{
			return NILP(cons) ? Qnil : ((List)cons).cdr;
		}

		public static Lisp.List CONS (Lisp.Object cons)
		{
			return ((List)cons);
		}

		public static bool NILP (Lisp.Object o)
		{
			return o == Qnil;
		}

		public static bool LISTP (Lisp.Object o)
		{
			return o is List || o == Qnil;
		}

		public static bool CONSP (Lisp.Object o)
		{
			return o is List;
		}

		// string foo
		public static int SCHARS (Lisp.Object o)
		{
			return (o as String).native_string.Length;
		}

		public static char SREF (Lisp.Object o, int c)
		{
			return (o as String).native_string[c];
		}

		[LispBuiltin ("defun", MinArgs = 3, Unevalled = true, DocString = 
@"Define NAME as a function.
The definition is (lambda ARGLIST [DOCSTRING] BODY...).
See also the function `interactive'.
usage: (defun NAME ARGLIST [DOCSTRING] BODY...)")]
		public static Lisp.Object Fdefun (L l, Lisp.Object sym, Lisp.Object arglist, Lisp.Object body)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);

			Lisp.Object defn = L.make_list (L.Qlambda, arglist, body);

			Symbol s = (Symbol)sym;
			Symbol.Ffset (l, s, defn);
			// XXX more here I'm sure...
			l.Environment = new List (new List(s, s), l.Environment);

			return sym;
		}

		[LispBuiltin ("setq", MinArgs = 2, Unevalled = true)]
		public static Lisp.Object Fsetq (L l, Lisp.Object sym, Lisp.Object val)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);

			var evalled = val.Eval(l);
			l.Environment = new List (new List(sym, evalled), l.Environment);
			return evalled;
		}
	}
}

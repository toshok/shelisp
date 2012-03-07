using System;
using System.Collections.Generic;
using System.Reflection;

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
		public Shelisp.Object Environment { get; set; }

		// the environment that contains all the builtin definitions
		static Shelisp.Object root_environment;

		public static Shelisp.Object Qcdr;
		public static Shelisp.Object Qcar;
		public static Shelisp.Object Qt;
		public static Shelisp.Object Qnil;
		public static Shelisp.Object Qlambda;
		public static Shelisp.Object Qunbound;
		public static Shelisp.Object Qquote;

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

		public static Shelisp.List make_list (params Shelisp.Object[] arr)
		{
			Object cons = Qnil;
			for (int i = arr.Length - 1; i >= 0; i --)
				cons = new List (arr[i], cons);
			return CONS(cons);
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

		[LispBuiltin ("defun", MinArgs = 3, Unevalled = true, DocString = 
@"Define NAME as a function.
The definition is (lambda ARGLIST [DOCSTRING] BODY...).
See also the function `interactive'.
usage: (defun NAME ARGLIST [DOCSTRING] BODY...)")]
		public static Shelisp.Object Fdefun (L l, Shelisp.Object sym, Shelisp.Object arglist, Shelisp.Object body)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);

			Shelisp.Object defn = L.make_list (L.Qlambda, arglist, body);

			Symbol s = (Symbol)sym;
			Symbol.Ffset (l, s, defn);
			// XXX more here I'm sure...
			l.Environment = new List (new List(s, s), l.Environment);

			return sym;
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
	}
}

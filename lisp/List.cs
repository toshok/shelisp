using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Lisp {
	public class List : Sequence {
		public List (Object car, Object cdr)
		{
			this.car = car;
			this.cdr = cdr;

			length = 1 + (!L.LISTP(cdr) ? 0 : (L.NILP(cdr) ? 0 : ((List)cdr).Length));
		}

		public override Lisp.Object Eval (L l, Lisp.Object env = null)
		{
			env = env ?? l.Environment;

			Debug.Print (this);

			// function evaluation gets stuffed in here unfortunately
			Object first = L.CAR(this);
			Object rest = L.CDR(this);

			Lisp.Object fun = first;

			if (first is Symbol) {
				fun = ((Symbol)first).function;
				Debug.Print ("first was a symbol, function = {0}", fun);
			}

			if (fun is Subr) {
				Subr subr = (Subr)fun;
				Lisp.Object[] args;

				if (!L.LISTP(rest))
					throw new WrongTypeArgumentException ("listp", rest);
				if (L.NILP (rest)) {
					args = new Lisp.Object[0];
				}
				else {
					Lisp.List rest_list = L.CONS(rest);
					args = new Lisp.Object[rest_list.Length];
					int i = 0;
					if (subr.unevalled) {
						Debug.Print ("   unevalled args");
					}
					else {
						Debug.Print ("   unevalled args");
					}
					foreach (var o in rest_list)
						args[i++] = subr.unevalled ? o : o.Eval (l, env);
				}

				return ((Subr)fun).Call (l, args);
			}
			else {
				if (fun.LispEq (L.Qunbound))
					throw new Exception (string.Format ("(void-function {0})", first));
				if (!L.LISTP (fun))
					throw new Exception (string.Format ("(invalid-function {0})", first));
				Lisp.Object funcar = L.CAR (fun);
				if (!(funcar is Symbol))
					throw new Exception (string.Format ("(invalid-function {0})", first));
#if notyet
				if (EQ (funcar, Qautoload)) {
					do_autoload (fun, original_fun);
					goto retry;
				}
				if (EQ (funcar, Qmacro))
					val = eval_sub (apply1 (Fcdr (fun), original_args));
				else if (EQ (funcar, Qlambda)
					 || EQ (funcar, Qclosure))
					val = apply_lambda (fun, original_args);
				else
					xsignal1 (Qinvalid_function, original_fun);
#else
				if (L.CAR (fun).LispEq (L.Qlambda)) {
					return ApplyLambda (l, fun, rest, env);
				}
				else
					throw new Exception (string.Format ("(invalid-function {0})", first));
#endif
			}
		}

		public Lisp.Object ApplyLambda (L l, Lisp.Object lambda, Lisp.Object args, Lisp.Object env = null)
		{
			if (env == null) env = l.Environment;

			Lisp.Object rest = L.CDR (lambda); // get rid of the lambda

			Lisp.Object arg_names = L.CAR(rest);

			Lisp.Object body = L.CAR (L.CDR (rest));

			/* evaluate each of the arguments in the current environment, then add them to the environment and evaluate the body of the lambda */
			Lisp.Object lexenv = env;

			IEnumerator<Lisp.Object> argname_enumerator = ((List)arg_names).GetEnumerator();
			IEnumerator<Lisp.Object> arg_enumerator = ((List)args).GetEnumerator();

			while (argname_enumerator.MoveNext()) {
				if (!arg_enumerator.MoveNext())
					throw new Exception ();

				lexenv = new List (new List (argname_enumerator.Current, arg_enumerator.Current.Eval (l, env)), lexenv);
			}

			Lisp.Object rv = body.Eval (l, lexenv);

			return rv;
		}

		public Object car;
		public Object cdr;

		int length;
		public override int Length {
			get { return length; }
		}

		public override IEnumerator<Lisp.Object> GetEnumerator ()
		{
			Object el = this;
			while (L.LISTP(el)) {
				if (L.NILP(el))
					break;
				yield return L.CAR(el);
				el = L.CDR(el);
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append("( ");
			foreach (var el in this) {
				sb.Append (el.ToString());
				sb.Append (" ");
			}
			sb.Append (")");
			return sb.ToString();
		}

		[LispBuiltin ("consp", MinArgs = 1)]
		public static Lisp.Object Fconsp (L l, Lisp.Object cons)
		{
			return L.CONSP(cons) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("atom", MinArgs = 1)]
		public static Lisp.Object Fatom (L l, Lisp.Object cons)
		{
			return L.CONSP(cons) ? L.Qnil : L.Qt;
		}

		[LispBuiltin ("listp", MinArgs = 1)]
		public static Lisp.Object Flistp (L l, Lisp.Object cons)
		{
			return L.LISTP(cons) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("nlistp", MinArgs = 1)]
		public static Lisp.Object Fnlistp (L l, Lisp.Object cons)
		{
			return L.LISTP(cons) ? L.Qnil : L.Qt;
		}

		[LispBuiltin ("null", MinArgs = 1)]
		public static Lisp.Object Fnull (L l, Lisp.Object cons)
		{
			return L.NILP(cons) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("cdr", MinArgs = 1)]
		public static Lisp.Object Fcdr (L l, Lisp.Object cons)
		{
			if (!L.LISTP (cons))
				throw new WrongTypeArgumentException ("listp", cons);
			if (L.NILP(cons))
				return L.Qnil;
			return L.CDR(cons);
		}

		[LispBuiltin ("car", MinArgs = 1)]
		public static Lisp.Object Fcar (L l, Lisp.Object cons)
		{
			if (!L.LISTP (cons))
				throw new WrongTypeArgumentException ("listp", cons);
			if (L.NILP(cons))
				return L.Qnil;
			return L.CAR(cons);
		}

		[LispBuiltin ("assq", MinArgs = 2)]
		public static Lisp.Object Fassq (L l, Lisp.Object key, Lisp.Object alist)
		{
			if (!L.LISTP(alist))
				throw new ArgumentException ("alist is not a list");

			foreach (var el in L.CONS(alist)) {
				if (!L.LISTP(el))
					continue;
				if (L.NILP(el))
					break;
				var car = L.CAR(el);
				// maybe inline Feq here and directly call key.LispEq (car)
				if (!L.NILP(Object.Feq (l, key, car)))
					return el;
			}
			return L.Qnil;
		}

		[LispBuiltin ("assoc", MinArgs = 2)]
		public static Lisp.Object Fassoc (L l, Lisp.Object key, Lisp.Object alist)
		{
			if (!L.LISTP(alist))
				throw new ArgumentException ("alist is not a list");

			foreach (var el in L.CONS(alist)) {
				if (!L.LISTP(el))
					continue;
				var car = L.CAR(el);
				// maybe inline Fequal here and directly call key.LispEqual (car)
				if (!L.NILP(Object.Fequal (l, key, car)))
					return el;
			}
			return L.Qnil;
		}
	}

}
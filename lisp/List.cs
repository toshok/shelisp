using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Shelisp {
	public class List : Sequence {
		public List (Object[] objs)
		{
			if (objs.Length == 0)
				throw new Exception ("this ctor can only be used with arrays of length >= 1");
			Object tail = L.Qnil;
			for (int i = objs.Length - 1; i >= 1; i --)
				tail = new List (objs[i], tail);
			cdr = tail;
			car = objs[0];
		}

		public List (Object car, Object cdr)
		{
			this.car = car;
			this.cdr = cdr;

			length = 1 + (!L.LISTP(cdr) ? 0 : (L.NILP(cdr) ? 0 : ((List)cdr).Length));
		}

		public override Shelisp.Object Eval (L l, Shelisp.Object env = null)
		{
			env = env ?? l.Environment;

			Debug.Print (this);

			// function evaluation gets stuffed in here unfortunately
			Object first = L.CAR(this);
			Object rest = L.CDR(this);

			Shelisp.Object fun = first;

			retry:

			if (first is Symbol) {
				Debug.Print ("first is {0}", first);
				Shelisp.Object lex_binding = List.Fassq (l, first, env);
				if (!L.NILP (lex_binding)) {
					first = L.CDR (lex_binding);
				}
				if (first is Symbol) {
					fun = L.Findirect_function (l, first);
					Debug.Print ("first was a symbol, function = {0}", fun);
				}
			}

			if (fun == null) {
				throw new Exception (string.Format ("fun is null, symbol is {0}", first));
			}

			if (fun is Subr) {
				Subr subr = (Subr)fun;
				Shelisp.Object[] args;

				if (!L.LISTP(rest))
					throw new WrongTypeArgumentException ("listp", rest);
				if (L.NILP (rest)) {
					args = new Shelisp.Object[0];
				}
				else {
					Shelisp.List rest_list = L.CONS(rest);
					args = new Shelisp.Object[rest_list.Length];
					int i = 0;
					if (subr.unevalled) {
						Debug.Print ("   unevalled args");
					}
					else {
						Debug.Print ("   evalled args");
					}
					foreach (var o in rest_list)
						args[i++] = subr.unevalled ? o : o.Eval (l, env);
				}

				try {
					return ((Subr)fun).Call (l, args);
				}
				catch {
					Console.WriteLine ("at lisp {0}", fun);
					throw;
				}
			}
			else {
				if (fun == null)
					throw new Exception (string.Format ("(void-function {0})", first));
				if (!L.LISTP (fun))
					throw new Exception (string.Format ("(invalid-function {0})", first));
				Shelisp.Object funcar = L.CAR (fun);
				if (!(funcar is Symbol))
					throw new Exception (string.Format ("(invalid-function {0})", first));
				if (funcar.LispEq (L.Qautoload)) {
					FileIO.DoAutoload (l, fun, first);
					goto retry;
				}
#if notyet
				if (EQ (funcar, Qmacro))
					val = eval_sub (apply1 (Fcdr (fun), original_args));
				else if (EQ (funcar, Qlambda)
					 || EQ (funcar, Qclosure))
					val = apply_lambda (fun, original_args);
				else
					xsignal1 (Qinvalid_function, original_fun);
#else
				if (funcar.LispEq (L.Qlambda)) {
					return ApplyLambda (l, fun, rest, env);
				}
				else if (funcar.LispEq (L.Qmacro)) {
					return ApplyLambda (l, L.CDR(fun), rest, env, false).Eval (l, env);
				}
				else
					throw new Exception (string.Format ("(invalid-function {0})", first));
#endif
			}
		}

		public Shelisp.Object ApplyLambda (L l, Shelisp.Object lambda, Shelisp.Object args, Shelisp.Object env = null, bool eval_args = true)
		{
			Debug.Print ("in ApplyLambda, lambda = {0}, args = {1}, eval_args = {2}", lambda, args, eval_args);
			if (env == null) env = l.Environment;

			Shelisp.Object rest = L.CDR (lambda); // get rid of the lambda

			Shelisp.Object arg_names = L.CAR(rest);

			Shelisp.Object body = L.CAR (L.CDR (rest));

			/* evaluate each of the arguments in the current environment, then add them to the environment and evaluate the body of the lambda */
			Shelisp.Object lexenv = env;

			if (L.CONSP (arg_names)) {
				IEnumerator<Shelisp.Object> argname_enumerator = ((List)arg_names).GetEnumerator();
				IEnumerator<Shelisp.Object> arg_enumerator = L.NILP (args) ? (new List<Shelisp.Object>()).GetEnumerator() : ((List)args).GetEnumerator();

				bool optional_seen = false;
				bool rest_seen = false;
				while (argname_enumerator.MoveNext()) {
					if (((Symbol)argname_enumerator.Current).name == "&optional") {
						optional_seen = true;
						continue;
					}
					if (((Symbol)argname_enumerator.Current).name == "&rest") {
						rest_seen = true;
						continue;
					}

					if (rest_seen) {
						// gather up the rest of the args into a single list, add it to the lexenv using the current argname,
						// and break out of the loop
						List<Shelisp.Object> list = new List<Shelisp.Object>();
						while (arg_enumerator.MoveNext())
							list.Add (eval_args ? arg_enumerator.Current.Eval (l, env) : arg_enumerator.Current);
						Shelisp.Object rest_args = list.Count == 0 ? L.Qnil : new List(list.ToArray());
						lexenv = new List (new List (argname_enumerator.Current, rest_args), lexenv);
						break;
					}
					else if (optional_seen) {
						// if there is nothing else in arg_enumerator, set parameters to nil
						if (!arg_enumerator.MoveNext()) {
							lexenv = new List (new List (argname_enumerator.Current, L.Qnil), lexenv);
							continue;
						}
					}
					else {
						if (!arg_enumerator.MoveNext())
							throw new Exception ("not enough args");
					}

					lexenv = new List (new List (argname_enumerator.Current, eval_args ? arg_enumerator.Current.Eval (l, env) : arg_enumerator.Current), lexenv);
				}
			}

			var old_env = l.Environment;
			l.Environment = lexenv;

			try {
				Shelisp.Object rv = body.Eval (l, lexenv);

				l.Environment = old_env;

				return rv;
			}
			catch (Exception e) {
				Console.WriteLine ("at lisp {0}", body);
				throw;
			}

		}

		public Object car;
		public Object cdr;

		int length;
		public override int Length {
			get { return length; }
		}

		public override IEnumerator<Shelisp.Object> GetEnumerator ()
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

		[LispBuiltin ("list", MinArgs = 0)]
		public static Shelisp.Object Flist (L l, params Shelisp.Object[] rest)
		{
			return new List (rest);
		}

		[LispBuiltin ("nconc", MinArgs = 1)]
		public static Shelisp.Object Fnconc (L l, params Shelisp.Object[] rest)
		{
			if (rest.Length == 0)
				return L.Qnil;

			var tail = rest[rest.Length - 1];
			for (int i = rest.Length - 2; i >= 0; i --) {
				var before = rest[i];
				if (L.CONSP (before)) {
					List head = (List)before;
					while (!L.Qnil.LispEq (head.cdr))
						head = (List)head.cdr;
					// destructively update head so that the cdr points at tail
					head.cdr = tail;
					tail = rest[i];
				}
			}
			return tail;

		}

		[LispBuiltin ("cons", MinArgs = 2)]
		public static Shelisp.Object Fcons (L l, Shelisp.Object car, Shelisp.Object cdr)
		{
			return new List (car, cdr);
		}

		[LispBuiltin ("consp", MinArgs = 1)]
		public static Shelisp.Object Fconsp (L l, Shelisp.Object cons)
		{
			return L.CONSP(cons) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("atom", MinArgs = 1)]
		public static Shelisp.Object Fatom (L l, Shelisp.Object cons)
		{
			return L.CONSP(cons) ? L.Qnil : L.Qt;
		}

		[LispBuiltin ("listp", MinArgs = 1)]
		public static Shelisp.Object Flistp (L l, Shelisp.Object cons)
		{
			return L.LISTP(cons) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("nlistp", MinArgs = 1)]
		public static Shelisp.Object Fnlistp (L l, Shelisp.Object cons)
		{
			return L.LISTP(cons) ? L.Qnil : L.Qt;
		}

		[LispBuiltin ("null", MinArgs = 1)]
		public static Shelisp.Object Fnull (L l, Shelisp.Object cons)
		{
			return L.NILP(cons) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("member", MinArgs = 2)]
		public static Shelisp.Object Fnull (L l, Shelisp.Object obj, Shelisp.Object list)
		{
			// check @list arg type
			while (L.CONSP (list)) {
				if (obj.LispEqual (L.CAR(list)))
					return list;
				list = L.CDR (list);
			}

			return L.Qnil;
		}

		[LispBuiltin ("cdr", MinArgs = 1)]
		public static Shelisp.Object Fcdr (L l, Shelisp.Object cons)
		{
			if (!L.LISTP (cons))
				throw new WrongTypeArgumentException ("listp", cons);
			if (L.NILP(cons))
				return L.Qnil;
			return L.CDR(cons);
		}

		[LispBuiltin ("car", MinArgs = 1)]
		public static Shelisp.Object Fcar (L l, Shelisp.Object cons)
		{
			if (!L.LISTP (cons))
				throw new WrongTypeArgumentException ("listp", cons);
			if (L.NILP(cons))
				return L.Qnil;
			return L.CAR(cons);
		}

		[LispBuiltin ("assq", MinArgs = 2)]
		public static Shelisp.Object Fassq (L l, Shelisp.Object key, Shelisp.Object alist)
		{
			if (!L.LISTP(alist))
				return L.Qnil;
			//throw new WrongTypeArgumentException ("listp", alist);

			foreach (var el in L.CONS(alist)) {
				if (!L.LISTP(el))
					continue;
				if (L.NILP(el))
					break;
				var car = L.CAR(el);
				if (key.LispEq (car))
					return el;
			}
			return L.Qnil;
		}

		[LispBuiltin ("assoc", MinArgs = 2)]
		public static Shelisp.Object Fassoc (L l, Shelisp.Object key, Shelisp.Object alist)
		{
			if (!L.LISTP(alist))
				throw new WrongTypeArgumentException ("listp", alist);

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
		
		public static Shelisp.Object reverse (Shelisp.Object alist)
		{
			return reverse (alist, L.Qnil);
		}

		private static Shelisp.Object reverse (Shelisp.Object alist, Shelisp.Object tail)
		{
			if (L.NILP (alist))
				return tail;
			else {
				Shelisp.List l = ((Shelisp.List)alist);
				return reverse (l.cdr, new List (l.car, tail));
			}
		}

		[LispBuiltin ("reverse", MinArgs = 1)]
		public static Shelisp.Object Freverse (L l, Shelisp.Object alist)
		{
			if (!L.LISTP(alist))
				throw new WrongTypeArgumentException ("listp", alist);

			return reverse ((Shelisp.List)alist, L.Qnil);
		}

		[LispBuiltin ("memq", MinArgs = 2)]
		public static Shelisp.Object Fmemq (L l, Shelisp.Object obj, Shelisp.Object alist)
		{
			if (!L.LISTP(alist))
				throw new WrongTypeArgumentException ("listp", alist);

			foreach (var el in (Shelisp.List)alist)
				if (el.LispEq (obj))
					return el;
			return L.Qnil;
		}

		static Shelisp.Object delq (Shelisp.Object obj, Shelisp.List alist)
		{
			if (alist.car.LispEq (obj))
				if (L.NILP (alist.cdr))
					return L.Qnil;
				else if (L.CONSP (alist.cdr))
					return delq (obj, (List)alist.cdr);
				else
					return alist.cdr;
			else {
				if (L.CONSP (alist.cdr))
					alist.cdr = delq (obj, (List)alist.cdr);
				return alist;
			}
		}

		[LispBuiltin ("delq", MinArgs = 2)]
		public static Shelisp.Object Fdelq (L l, Shelisp.Object obj, Shelisp.Object alist)
		{
			if (!L.LISTP(alist))
				return L.Qnil;
			//throw new WrongTypeArgumentException ("listp", alist);

			return delq (obj, (List) alist);
		}


		[LispBuiltin ("nthcdr", MinArgs = 2)]
		public static Shelisp.Object Fnthcdr (L l, Shelisp.Object n, Shelisp.Object alist)
		{
			if (!L.LISTP(alist))
				throw new WrongTypeArgumentException ("listp", alist);
			if (!(n is Number) || !(((Number)n).boxed is int))
				throw new WrongTypeArgumentException ("integerp", n);

			int n_i = (int)((Number)n).boxed;

			if (n_i <= 0)
				return alist;

			while (n_i > 0) {
				alist = ((List)alist).cdr;
				if (L.NILP (alist))
					break;
			}

			if (n_i > 0)
				return L.Qnil;

			return alist;
		}

		[LispBuiltin ("safe-length", MinArgs = 1)]
		public static Shelisp.Object Fnthcdr (L l, Shelisp.Object alist)
		{
			if (!L.CONSP(alist))
				return L.Qnil;

			Dictionary<Shelisp.Object,bool> conses = new Dictionary<Shelisp.Object,bool>();

			Shelisp.List cons;
			do {
				cons = (Shelisp.List)alist;
				if (conses.ContainsKey (cons))
					break;
				conses[cons] = true;
			} while (!L.NILP (cons));

			return new Number(conses.Keys.Count);
		}
	}

}
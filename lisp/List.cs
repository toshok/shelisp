using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Shelisp {
	public class List : Sequence {
		public List (params Object[] objs)
		{
			if (objs.Length == 0)
				throw new Exception ("this ctor can only be used with arrays of length >= 1");
			Object tail = L.Qnil;
			for (int i = objs.Length - 1; i >= 1; i --)
				tail = new List (objs[i] ?? L.Qnil, tail);
			cdr = tail;
			car = objs[0] ?? L.Qnil;
		}

		public List (Object car, Object cdr)
		{
			this.car = car;
			this.cdr = cdr;
		}

		public override int GetHashCode ()
		{
			int hash = 0;
			foreach (var o in this) {
				if (hash == 0)
					hash = o.GetHashCode();
				else
					hash = hash * 16 + o.GetHashCode();
			}
			return hash;
		}

		public override Shelisp.Object Eval (L l, Shelisp.Object env = null)
		{
			try {
				if (l.eval_depth ++ >= L.max_lisp_eval_depth)
					throw new Exception ("max eval depth exceeded");

				env = env ?? l.Environment;

				Debug.Print (this);

				// function evaluation gets stuffed in here unfortunately
				Object first = L.CAR(this);
				Object rest = L.CDR(this);

				Shelisp.Object fun = first;

				retry:

#if my_stupid_way
				if (first is Symbol) {
					Debug.Print ("first is {0}", first);
					Shelisp.Object lex_binding = List.Fassq (l, first, env);
					if (!L.NILP (lex_binding)) {
						var binding = L.CDR (lex_binding);
						if (binding is Symbol && !(L.Qunbound.LispEq (((Symbol)binding).Function)))
							fun = ((Symbol)binding).Function;
					}

					if (fun is Symbol) {
						fun = L.Findirect_function (l, fun);
						Debug.Print ("first was a symbol, function = {0}", fun);
					}
				}
#else
				fun = first;
				if ((fun is Symbol) && !fun.LispEq (L.Qunbound)) {
					fun = ((Symbol)fun).Function;
					if (fun is Symbol)
						fun = L.Findirect_function (l, fun, L.Qnil);
				}
#endif

				if (fun is Subr) {
					Subr subr = (Subr)fun;
					Shelisp.Object[] args;

					L.EvalSpew ("evaluating subr application, {0}", fun);

					if (!L.LISTP(rest))
						throw new WrongTypeArgumentException ("listp", rest);
					if (L.NILP (rest)) {
						args = new Shelisp.Object[0];
					}
					else {
						Shelisp.List rest_list = L.CONS(rest);
						args = new Shelisp.Object[rest_list.Length];
						int argnum = 0;
						if (subr.unevalled) {
							Debug.Print ("   unevalled args");
						}
						else {
							Debug.Print ("   evalled args");
						}

						L.EvalIndent(1);
						foreach (var o in rest_list) {
							L.EvalSpew ("arg: {0}", o);
							if (subr.unevalled) {
								args[argnum++] = o;
							}
							else {
								var evalled = o.Eval (l, env);
								if (L.Qunbound.LispEq (evalled))
									throw new Exception (string.Format ("form {0} evaluated to unbound.  this is NOT what you want", o));
								args[argnum++] = evalled;
							}
						}
						L.EvalOutdent(1);
					}

					try {
						L.EvalIndent();
						Shelisp.Object rv = ((Subr)fun).Call (l, args);
						L.EvalOutdent();
						L.EvalSpew ("evaluating of {0} resulted in {1}", fun, rv);
						return rv;
					}
					catch {
						Debug.Print ("at lisp {0}", fun);
						throw;
					}
				}
				else {
					if (L.NILP (fun) || fun.LispEq(L.Qunbound)) {
						Console.WriteLine ("entire application is {0}", this);
						throw new LispVoidFunctionException (first);
					}
					if (!L.LISTP (fun))
						throw new LispInvalidFunctionException (first);
					Shelisp.Object funcar = L.CAR (fun);
					if (!(funcar is Symbol)) {
						throw new LispInvalidFunctionException (first);
					}
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
					if (funcar.LispEq (L.Qmacro)) {
						L.EvalSpew ("evaluating macro application, {0} = {1}, ", first, L.CDR(fun));
						L.EvalIndent(1);
						foreach (var r in (List)rest)
							L.EvalSpew ("arg: {0}", r);
						L.EvalOutdent(1);

						L.EvalIndent();
						var expanded = List.ApplyLambda (l, L.CDR(fun), rest, env, false);
						L.EvalOutdent();

						L.EvalSpew ("macro {0} expanded to {1}", first, expanded);

						var expanded_evalled = expanded.Eval (l, env);
						L.EvalSpew ("evaluating of {0} resulted in {1}", first, expanded_evalled);

						return expanded_evalled;
					}
					if (funcar.LispEq (L.Qlambda)
					    || funcar.LispEq (L.Qclosure)) {

						L.EvalSpew ("evaluating function application, {0} = {1}, ", first, L.CDR(fun));
						if (L.CONSP(rest)) {
							L.EvalIndent (1);
							foreach (var r in (List)rest)
								L.EvalSpew ("arg: {0}", r);
							L.EvalOutdent (1);
						}

						L.EvalIndent();
						var rv = List.ApplyLambda (l, fun, rest, env);
						L.EvalOutdent();

						L.EvalSpew ("evaluating of {0} resulted in {1}", first, rv);

						return rv;
					}
					else {
						throw new LispInvalidFunctionException (first);
					}
#endif
				}
			}
			finally {
				l.eval_depth --;
			}
		}

		public static Shelisp.Object ApplyLambda (L l, Shelisp.Object fun, Shelisp.Object args, [LispOptional] Shelisp.Object env, bool eval_args = true)
		{
			try {
				if (l.eval_depth ++ >= L.max_lisp_eval_depth)
					throw new Exception ("max eval depth exceeded");

				// 			Console.WriteLine ("in ApplyLambda, fun = {0}, args = {1}, eval_args = {2}", fun, args, eval_args);
				// 			Console.WriteLine (Environment.StackTrace);

				if (env == null) env = l.Environment;

				if (!L.CONSP(fun))
					Console.WriteLine ("{0} is not a cons cell!  we're about to die!", fun);

				if (L.CAR(fun).LispEq (L.Qclosure)) {
					// this sets fun = ( environment args rest... ) where args rest... came from the original lambda.
					// so the cdr below meant to skip the lambda actually skips the environment.
					fun = L.CDR(fun);
					env = L.CAR(fun);
				}

				Shelisp.Object rest = L.CDR (fun); // get rid of the lambda

				Shelisp.Object arg_names = L.CAR(rest);

				Shelisp.Object body = L.CDR (rest);

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
					Shelisp.Object rv = L.Qnil;
					foreach (var form in (List)body) {
						L.EvalIndent();
						rv = form.Eval (l, lexenv);
						L.EvalOutdent();
					}

					return rv;
				}
				catch (Exception) {
					Debug.Print ("at lisp {0}", body);
					throw;
				}
				finally {
					l.Environment = old_env;
				}
			}
			finally {
				l.eval_depth --;
			}
		}

		private Object _car;
		public Object car {
			get { return _car; }
			set { if (value == null) throw new ArgumentNullException (); _car = value; }
		}

		private Object _cdr;
		public Object cdr {
			get { return _cdr; }
			set { if (value == null) throw new ArgumentNullException (); _cdr = value; }
		}

		public override int Length {
			get {
				Shelisp.Object tail = this;
				int length = 0;
				while (L.CONSP (tail)) {
					length++;
					if (!L.CONSP (((List)tail).cdr))
						break;
					tail = ((List)tail).cdr;
				}
				return length;
			}
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

		public override string ToString(string format_type)
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append("(");
			Shelisp.Object el = this;
			do {
				Shelisp.Object car = L.CAR(el);
				Shelisp.Object cdr = L.CDR(el);

				sb.Append (car.ToString(format_type));
				if (L.NILP (cdr))
					break;
				else {
					if (!L.CONSP (cdr))
						sb.AppendFormat (" . {0}", cdr.ToString(format_type));
					else {
						sb.Append (" ");
					}
				}
				el = cdr;
			} while (L.CONSP (el));

			sb.Append (")");
			return sb.ToString();
		}

		[LispBuiltin]
		public static Shelisp.Object Flist (L l, params Shelisp.Object[] rest)
		{
			if (rest.Length == 0)
				return L.Qnil;
			return new List (rest);
		}

		[LispBuiltin]
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

		[LispBuiltin]
		public static Shelisp.Object Fcons (L l, Shelisp.Object car, Shelisp.Object cdr)
		{
			return new List (car, cdr);
		}

		[LispBuiltin]
		public static Shelisp.Object Fconsp (L l, Shelisp.Object cons)
		{
			return L.CONSP(cons) ? L.Qt : L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fatom (L l, Shelisp.Object cons)
		{
			return L.CONSP(cons) ? L.Qnil : L.Qt;
		}

		[LispBuiltin]
		public static Shelisp.Object Flistp (L l, Shelisp.Object cons)
		{
			return L.LISTP(cons) ? L.Qt : L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fnlistp (L l, Shelisp.Object cons)
		{
			return L.LISTP(cons) ? L.Qnil : L.Qt;
		}

		[LispBuiltin]
		public static Shelisp.Object Fnull (L l, Shelisp.Object cons)
		{
			return L.NILP(cons) ? L.Qt : L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fmember (L l, Shelisp.Object obj, Shelisp.Object list)
		{
			if (!L.LISTP (list))
				throw new WrongTypeArgumentException ("listp", list);

			while (L.CONSP (list)) {
				if (obj.LispEqual (L.CAR(list)))
					return list;
				list = L.CDR (list);
			}

			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fcdr (L l, Shelisp.Object cons)
		{
			if (!L.LISTP (cons))
				throw new WrongTypeArgumentException ("listp", cons);
			if (L.NILP(cons))
				return L.Qnil;
			return L.CDR(cons);
		}

		[LispBuiltin]
		public static Shelisp.Object Fcdr_safe (L l, Shelisp.Object cons)
		{
			if (!L.CONSP (cons))
				return L.Qnil;
			return L.CDR(cons);
		}

		[LispBuiltin]
		public static Shelisp.Object Fcar (L l, Shelisp.Object cons)
		{
			if (!L.LISTP (cons))
				throw new WrongTypeArgumentException ("listp", cons);
			if (L.NILP(cons))
				return L.Qnil;
			return L.CAR(cons);
		}

		[LispBuiltin]
		public static Shelisp.Object Fcar_safe (L l, Shelisp.Object cons)
		{
			if (!L.CONSP (cons))
				return L.Qnil;
			return L.CAR(cons);
		}

		[LispBuiltin]
		public static Shelisp.Object Fassq (L l, Shelisp.Object key, Shelisp.Object alist)
		{
			if (!L.CONSP(alist))
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

		[LispBuiltin]
		public static Shelisp.Object Frassq (L l, Shelisp.Object key, Shelisp.Object alist)
		{
			if (!L.CONSP(alist))
				return L.Qnil;
			//throw new WrongTypeArgumentException ("listp", alist);

			foreach (var el in L.CONS(alist)) {
				if (!L.LISTP(el))
					continue;
				if (L.NILP(el))
					break;
				var cdr = L.CDR(el);
				if (key.LispEq (cdr))
					return el;
			}
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fassoc (L l, Shelisp.Object key, Shelisp.Object alist)
		{
			if (!L.LISTP(alist))
				throw new WrongTypeArgumentException ("listp", alist);

			if (L.NILP(alist))
				return L.Qnil;

			foreach (var el in L.CONS(alist)) {
				if (!L.LISTP(el))
					continue;
				var car = L.CAR(el);
				if (key.LispEqual (car))
					return el;
			}
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Frassoc (L l, Shelisp.Object val, Shelisp.Object alist)
		{
			if (!L.LISTP(alist))
				throw new WrongTypeArgumentException ("listp", alist);

			if (L.NILP(alist))
				return L.Qnil;

			foreach (var el in L.CONS(alist)) {
				if (!L.LISTP(el))
					continue;
				var cdr = L.CDR(el);
				if (val.LispEqual (cdr))
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

		[LispBuiltin]
		public static Shelisp.Object Freverse (L l, Shelisp.Object alist)
		{
			if (!L.LISTP(alist))
				throw new WrongTypeArgumentException ("listp", alist);

			return reverse (alist, L.Qnil);
		}

		private static Shelisp.Object nreverse (Shelisp.Object alist)
		{
			Shelisp.Object el = alist;
			Shelisp.Object prev = L.Qnil;
			while (!L.NILP (el)) {
				var next = L.CDR(el);

				((List)el).cdr = prev;

				prev = el;
				el = next;
			}

			return prev;
		}

		[LispBuiltin]
		public static Shelisp.Object Fnreverse (L l, Shelisp.Object alist)
		{
			if (!L.LISTP(alist))
				throw new WrongTypeArgumentException ("listp", alist);

			if (L.NILP (alist))
				return L.Qnil;

			return nreverse (alist);
		}

		public static Shelisp.Object Memq (Shelisp.Object obj, Shelisp.Object alist)
		{
			Shelisp.Object tail = alist;

			do {
				if (obj.LispEq (L.CAR(tail)))
					return tail;
				tail = L.CDR(tail);
			} while (L.CONSP (tail));

			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fmemq (L l, Shelisp.Object obj, Shelisp.Object alist)
		{
			if (!L.LISTP(alist))
				throw new WrongTypeArgumentException ("listp", alist);

			return L.NILP(alist) ? L.Qnil : Memq (obj, (Shelisp.List)alist);
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

		[LispBuiltin]
		public static Shelisp.Object Fdelq (L l, Shelisp.Object obj, Shelisp.Object alist)
		{
			if (!L.CONSP(alist))
				return L.Qnil;
			//throw new WrongTypeArgumentException ("listp", alist);

			return delq (obj, (List) alist);
		}


		[LispBuiltin]
		public static Shelisp.Object Fnthcdr (L l, Shelisp.Object n, Shelisp.Object alist)
		{
			if (!L.LISTP(alist))
				throw new WrongTypeArgumentException ("listp", alist);
			if (!Number.IsInt (n))
				throw new WrongTypeArgumentException ("integerp", n);

			int n_i = (int)((Number)n).boxed;

			if (n_i <= 0)
				return alist;

			while (n_i > 0) {
				alist = ((List)alist).cdr;
				if (L.NILP (alist))
					break;
				n_i --;
			}

			if (n_i > 0)
				return L.Qnil;

			return alist;
		}

		[LispBuiltin]
		public static Shelisp.Object Fsafe_length (L l, Shelisp.Object alist)
		{
			if (!L.CONSP(alist))
				return L.Qnil;

			Dictionary<Shelisp.Object,bool> conses = new Dictionary<Shelisp.Object,bool>();

			Shelisp.Object cons = alist;
			do {
				if (conses.ContainsKey (cons))
					break;
				conses[cons] = true;
				cons = L.CDR(cons);
			} while (L.CONSP (cons));

			return new Number(conses.Keys.Count);
		}

		[LispBuiltin (DocString = "Set the cdr of CELL to be NEWCDR.  Returns NEWCDR.")]
		public static Shelisp.Object Fsetcdr (L l, Shelisp.Object cell, Shelisp.Object newcdr)
		{
			if (!L.CONSP(cell))
				throw new WrongTypeArgumentException ("consp", cell);

			((List)cell).cdr = newcdr;
			return newcdr;
		}

		[LispBuiltin (DocString = "Set the car of CELL to be NEWCAR.  Returns NEWCAR.")]
		public static Shelisp.Object Fsetcar (L l, Shelisp.Object cell, Shelisp.Object newcar)
		{
			if (!L.CONSP(cell))
				throw new WrongTypeArgumentException ("consp", cell);

			((List)cell).car = newcar;
			return newcar;
		}

		[LispBuiltin (DocString = @"Sort LIST, stably, comparing elements using PREDICATE.
Returns the sorted list.  LIST is modified by side effects.
PREDICATE is called with two elements of LIST, and should return non-nil
if the first element should sort before the second.")]
		public static Shelisp.Object Fsort (L l, Shelisp.Object list, Shelisp.Object predicate)
		{
			Shelisp.Object front, back;
			Shelisp.Object tem;
			int length;

			front = list;
			length = ((List)list).Length;
			if (length < 2)
				return list;

			length = length / 2 - 1;
			tem = Fnthcdr (l, new Number(length), list);
			back = Fcdr (l, tem);
			Fsetcdr (l, tem, L.Qnil);

			front = Fsort (l, front, predicate);
			back = Fsort (l, back, predicate);

			return merge (l, front, back, predicate);
		}

		private static Shelisp.Object merge (L l, Shelisp.Object org_l1, Shelisp.Object org_l2, Shelisp.Object pred)
		{
			Shelisp.Object value;
			Shelisp.Object tail;
			Shelisp.Object tem;
			Shelisp.Object l1, l2;

			l1 = org_l1;
			l2 = org_l2;
			tail = L.Qnil;
			value = L.Qnil;

			while (true) {
				if (L.NILP (l1)) {
					if (L.NILP (tail))
						return l2;
					Fsetcdr (l, tail, l2);
					return value;
				}
				if (L.NILP (l2)) {
					if (L.NILP (tail))
						return l1;
					Fsetcdr (l, tail, l1);
					return value;
				}
				tem = new List (new Shelisp.Object[] { pred, Fcar (l, l2), Fcar (l, l1) }).Eval (l);
				if (L.NILP (tem)) {
					tem = l1;
					l1 = Fcdr (l, l1);
					org_l1 = l1;
				}
				else {
					tem = l2;
					l2 = Fcdr (l, l2);
					org_l2 = l2;
				}
				if (L.NILP (tail))
					value = tem;
				else
					Fsetcdr (l, tail, tem);
				tail = tem;
			}
		}

	}

}
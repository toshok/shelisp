using System;
using System.Collections;
using System.Collections.Generic;

namespace Lisp {
	static class Control {

		[LispBuiltin ("progn", MinArgs = 0, Unevalled = true)]
		public static Lisp.Object Fprogn(L l, params Lisp.Object[] forms)
		{
			Lisp.Object rv = L.Qnil;
			for (int i = 0; i < forms.Length; i ++)
				rv = forms[i].Eval(l);

			return rv;
		}

		[LispBuiltin ("prog1", MinArgs = 1, Unevalled = true)]
		public static Lisp.Object Fprog1(L l, Lisp.Object form1, params Lisp.Object[] forms)
		{
			Lisp.Object rv = form1.Eval(l);
			for (int i = 0; i < forms.Length; i ++)
				forms[i].Eval(l);
			return rv;
		}

		[LispBuiltin ("prog2", MinArgs = 1, Unevalled = true)]
		public static Lisp.Object Fprog1(L l, Lisp.Object form1, Lisp.Object form2, params Lisp.Object[] forms)
		{
			Lisp.Object rv;

			form1.Eval(l);
			rv = form2.Eval(l);

			for (int i = 0; i < forms.Length; i ++)
				forms[i].Eval(l);

			return rv;
		}

		[LispBuiltin ("if", MinArgs = 2, Unevalled = true)]
		public static Lisp.Object Fif(L l, Lisp.Object condition, Lisp.Object then_form, params Lisp.Object[] else_forms)
		{
			if (!L.NILP(condition.Eval(l)))
				return then_form.Eval(l);
			else {
				Lisp.Object rv = L.Qnil;
				for (int i = 0; i < else_forms.Length; i ++)
					rv = else_forms[i].Eval(l);
				return rv;
			}
		}

		[LispBuiltin ("cond", MinArgs = 2, Unevalled = true)]
		public static Lisp.Object Fcond(L l, params Lisp.Object[] clauses)
		{
			for (int i = 0; i < clauses.Length; i ++) {
				Lisp.Object clause = clauses[i];
				Lisp.Object condition = L.CAR(clause);

				Lisp.Object rv = condition.Eval (l);
				if (!L.NILP(rv)) {
					Lisp.Object body_forms = L.CDR(clause);
					if (!L.NILP (body_forms)) {
						foreach (var form in (List)body_forms)
							rv = form.Eval(l);
					}

					return rv;
				}
				
			}

			return L.Qnil;
		}


		[LispBuiltin ("not", MinArgs = 1, Unevalled = true)]
		public static Lisp.Object Fnot (L l, Lisp.Object cond)
		{
			return L.NILP(cond) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("or", MinArgs = 0, Unevalled = true)]
		public static Lisp.Object For (L l, params Lisp.Object[] args)
		{
			for (int i = 0; i < args.Length; i ++) {
				Lisp.Object evalled = args[i].Eval (l);
				if (!L.NILP (evalled))
					return evalled;
			}
			return L.Qnil;
		}

		[LispBuiltin ("and", MinArgs = 0, Unevalled = true)]
		public static Lisp.Object Fand (L l, params Lisp.Object[] args)
		{
			Lisp.Object evalled = L.Qt;

			for (int i = 0; i < args.Length; i ++) {
				evalled = args[i].Eval (l);
				if (L.NILP (evalled))
					return evalled;
			}
			return evalled;
		}

		[LispBuiltin ("while", MinArgs = 1, Unevalled = true)]
		public static Lisp.Object Fand (L l, Lisp.Object condition, params Lisp.Object[] forms)
		{
			while (!L.NILP (condition.Eval (l))) {
				for (int i = 0; i < forms.Length; i ++)
					forms[i].Eval(l);
			}

			return L.Qnil;
		}


	}
}
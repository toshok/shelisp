using System;
using System.Collections;
using System.Collections.Generic;

namespace Shelisp {
	public static class Control {

		[LispBuiltin ("progn", MinArgs = 0, Unevalled = true)]
		public static Shelisp.Object Fprogn(L l, params Shelisp.Object[] forms)
		{
			Shelisp.Object rv = L.Qnil;
			for (int i = 0; i < forms.Length; i ++)
				rv = forms[i].Eval(l);

			return rv;
		}

		[LispBuiltin ("prog1", MinArgs = 1, Unevalled = true)]
		public static Shelisp.Object Fprog1(L l, Shelisp.Object form1, params Shelisp.Object[] forms)
		{
			Shelisp.Object rv = form1.Eval(l);
			for (int i = 0; i < forms.Length; i ++)
				forms[i].Eval(l);
			return rv;
		}

		[LispBuiltin ("prog2", MinArgs = 1, Unevalled = true)]
		public static Shelisp.Object Fprog1(L l, Shelisp.Object form1, Shelisp.Object form2, params Shelisp.Object[] forms)
		{
			Shelisp.Object rv;

			form1.Eval(l);
			rv = form2.Eval(l);

			for (int i = 0; i < forms.Length; i ++)
				forms[i].Eval(l);

			return rv;
		}

		[LispBuiltin ("if", MinArgs = 2, Unevalled = true)]
		public static Shelisp.Object Fif(L l, Shelisp.Object condition, Shelisp.Object then_form, params Shelisp.Object[] else_forms)
		{
			if (!L.NILP(condition.Eval(l)))
				return then_form.Eval(l);
			else {
				Shelisp.Object rv = L.Qnil;
				for (int i = 0; i < else_forms.Length; i ++)
					rv = else_forms[i].Eval(l);
				return rv;
			}
		}

		[LispBuiltin ("cond", MinArgs = 2, Unevalled = true)]
		public static Shelisp.Object Fcond(L l, params Shelisp.Object[] clauses)
		{
			for (int i = 0; i < clauses.Length; i ++) {
				Shelisp.Object clause = clauses[i];
				Shelisp.Object condition = L.CAR(clause);

				Shelisp.Object rv = condition.Eval (l);
				if (!L.NILP(rv)) {
					Shelisp.Object body_forms = L.CDR(clause);
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
		public static Shelisp.Object Fnot (L l, Shelisp.Object cond)
		{
			return L.NILP(cond) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("or", MinArgs = 0, Unevalled = true)]
		public static Shelisp.Object For (L l, params Shelisp.Object[] args)
		{
			for (int i = 0; i < args.Length; i ++) {
				Shelisp.Object evalled = args[i].Eval (l);
				if (!L.NILP (evalled))
					return evalled;
			}
			return L.Qnil;
		}

		[LispBuiltin ("and", MinArgs = 0, Unevalled = true)]
		public static Shelisp.Object Fand (L l, params Shelisp.Object[] args)
		{
			Shelisp.Object evalled = L.Qt;

			for (int i = 0; i < args.Length; i ++) {
				evalled = args[i].Eval (l);
				if (L.NILP (evalled))
					return evalled;
			}
			return evalled;
		}

		[LispBuiltin ("while", MinArgs = 1, Unevalled = true)]
		public static Shelisp.Object Fand (L l, Shelisp.Object condition, params Shelisp.Object[] forms)
		{
			while (!L.NILP (condition.Eval (l))) {
				for (int i = 0; i < forms.Length; i ++)
					forms[i].Eval(l);
			}

			return L.Qnil;
		}


	}
}
using System;
using System.Collections;
using System.Collections.Generic;

namespace Shelisp {
	public abstract class Array : Sequence {
		public abstract Shelisp.Object this[int index] { get; set; }

		[LispBuiltin ("arrayp", MinArgs = 1)]
		public static Shelisp.Object Farrayp(L l, Shelisp.Object o)
		{
			return (o is Array) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("aref", MinArgs = 2)]
		public static Shelisp.Object Farrayp(L l, Shelisp.Object arr, Shelisp.Object idx)
		{
			// XXX type checks
			return ((Array)arr)[(int)(Number)idx];
		}

		[LispBuiltin ("aset", MinArgs = 3, Unevalled = true)]
		public static Shelisp.Object Faset(L l, Shelisp.Object sym, Shelisp.Object idx, Shelisp.Object val)
		{
			Shelisp.Object arr = sym.Eval (l).Eval(l);
			idx = idx.Eval(l);
			val = val.Eval(l);

			if (arr is Vector) {
				return ((Vector)arr)[(int)(Number)idx] = val;
			}
			else if (arr is Array) {
				return ((Array)arr)[(int)(Number)idx] = val;
			}
			else {
				throw new WrongTypeArgumentException ("array-or-vectorp", sym);
			}
		}

		[LispBuiltin ("fillarray", MinArgs = 2)]
		public static Shelisp.Object Ffillarray(L l, Shelisp.Object arr, Shelisp.Object val)
		{
			// XXX type checks
			Array array = (Array)arr;
			for (int i = 0; i < array.Length; i ++) {
				array[i] = val;
			}
			return arr;
		}
	}
}

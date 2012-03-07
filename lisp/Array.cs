using System;
using System.Collections;
using System.Collections.Generic;

namespace Lisp {
	abstract class Array : Sequence {
		public abstract Lisp.Object this[int index] { get; set; }

		[LispBuiltin ("arrayp", MinArgs = 1)]
		public static Lisp.Object Farrayp(L l, Lisp.Object o)
		{
			return (o is Array) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("aref", MinArgs = 2)]
		public static Lisp.Object Farrayp(L l, Lisp.Object arr, Lisp.Object idx)
		{
			// XXX type checks
			return ((Array)arr)[(int)(Number)idx];
		}

		[LispBuiltin ("aset", MinArgs = 3)]
		public static Lisp.Object Faset(L l, Lisp.Object arr, Lisp.Object idx, Lisp.Object val)
		{
			// XXX type checks
			return ((Array)arr)[(int)(Number)idx] = val;
		}

		[LispBuiltin ("fillarray", MinArgs = 2)]
		public static Lisp.Object Ffillarray(L l, Lisp.Object arr, Lisp.Object val)
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

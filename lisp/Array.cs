using System;
using System.Collections;
using System.Collections.Generic;

namespace Shelisp {
	public abstract class Array : Sequence {
		public override int GetHashCode ()
		{
			int hash = Length;
			foreach (var o in this) {
				hash = hash * 16 + o.GetHashCode();
			}
			return hash;
		}

		public abstract Shelisp.Object this[int index] { get; set; }

		[LispBuiltin]
		public static Shelisp.Object Farrayp(L l, Shelisp.Object o)
		{
			return (o is Array) ? L.Qt : L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Faref(L l, Shelisp.Object arr, Shelisp.Object idx)
		{
			// XXX type checks
			return ((Array)arr)[(int)(Number)idx];
		}

		[LispBuiltin]
		public static Shelisp.Object Faset(L l, Shelisp.Object sym, Shelisp.Object idx, Shelisp.Object val)
		{
			if (sym is Vector) {
				return ((Vector)sym)[(int)(Number)idx] = val;
			}
			else if (sym is Array) {
				return ((Array)sym)[(int)(Number)idx] = val;
			}
			else {
				throw new WrongTypeArgumentException ("array-or-vectorp", sym ?? L.Qnil);
			}
		}

		[LispBuiltin]
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

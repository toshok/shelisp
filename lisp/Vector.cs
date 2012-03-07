using System;
using System.Collections.Generic;
using System.Text;

namespace Lisp {
	public class Vector : Array {
		public Vector (Lisp.Object[] data)
		{
			this.data = data;
		}

		Lisp.Object[] data;

		public override int Length {
			get { return data.Length; }
		}

		public override Lisp.Object this[int index]
		{
			get { return data[index]; }
			set { data[index] = value; }
		}

		public override IEnumerator<Lisp.Object> GetEnumerator ()
		{
			for (int i = 0; i < data.Length; i ++)
				yield return data[i];
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append("[ ");
			foreach (var el in this) {
				sb.Append (el.ToString());
				sb.Append (" ");
			}
			sb.Append ("]");
			return sb.ToString();
		}

		[LispBuiltin ("vectorp", MinArgs = 1)]
		public static Lisp.Object Fvectorp(L l, Lisp.Object o)
		{
			return (o is Vector) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("vector", MinArgs = 0)]
		public static Lisp.Object Fvector (L l, params Lisp.Object[] args)
		{
			return new Vector (args);
		}

		[LispBuiltin ("make-vector", MinArgs = 2)]
		public static Lisp.Object Fmake_vector (L l, Lisp.Object length, Lisp.Object val)
		{
			Lisp.Object[] vals = new Lisp.Object[(int)(Number)length];
			for (int i = 0; i < vals.Length; i ++)
				vals[i] = val;
			return new Vector (vals);
		}

		[LispBuiltin ("vconcat", MinArgs = 0)]
		public static Lisp.Object Fvconcat (L l, params Lisp.Object[] args)
		{
			throw new NotImplementedException ();
		}
	}
}
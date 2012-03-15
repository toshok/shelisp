using System;
using System.Collections.Generic;
using System.Text;

namespace Shelisp {
	public class Vector : Array {
		public Vector (Shelisp.Object[] data)
		{
			this.data = data;
		}

		public Vector (int length, Shelisp.Object initial)
		{
			data = new Shelisp.Object[length];
			for (int i = 0; i < length; i ++)
				data[i] = initial;
		}

		Shelisp.Object[] data;

		public override int Length {
			get { return data.Length; }
		}

		public override Shelisp.Object this[int index]
		{
			get { return data[index]; }
			set { data[index] = value; }
		}

		public override IEnumerator<Shelisp.Object> GetEnumerator ()
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
		public static Shelisp.Object Fvectorp(L l, Shelisp.Object o)
		{
			return (o is Vector) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("vector", MinArgs = 0)]
		public static Shelisp.Object Fvector (L l, params Shelisp.Object[] args)
		{
			return new Vector (args);
		}

		[LispBuiltin ("make-vector", MinArgs = 2)]
		public static Shelisp.Object Fmake_vector (L l, Shelisp.Object length, Shelisp.Object val)
		{
			Shelisp.Object[] vals = new Shelisp.Object[(int)(Number)length];
			for (int i = 0; i < vals.Length; i ++)
				vals[i] = val;
			return new Vector (vals);
		}

		[LispBuiltin ("vconcat", MinArgs = 0)]
		public static Shelisp.Object Fvconcat (L l, params Shelisp.Object[] args)
		{
			throw new NotImplementedException ();
		}
	}
}
namespace Lisp {
	public class Number : Object {
		public Number (object boxed)
		{
			this.boxed = boxed;
		}

		public override bool LispEq (Lisp.Object other)
		{
			if (!(other is Number))
				return false;
			// lisp numbers are eq if their values are equal
			return ((Lisp.Number)other).boxed == boxed;
		}

		public override bool LispEqual (Lisp.Object other)
		{
			if (!(other is Number))
				return false;
			// lisp numbers are equal if their values are equal
			return ((Lisp.Number)other).boxed == boxed;
		}

		public object boxed;

		public override string ToString()
		{
			return boxed.ToString();
		}

		public static implicit operator byte (Number num)
		{
			return (byte)num.boxed;
		}

		public static implicit operator char (Number num)
		{
			return (char)num.boxed;
		}

		public static implicit operator short (Number num)
		{
			return (short)num.boxed;
		}

		public static implicit operator int (Number num)
		{
			return (int)num.boxed;
		}

		public static implicit operator long (Number num)
		{
			return (long)num.boxed;
		}

		public static implicit operator float (Number num)
		{
			return (float)num.boxed;
		}

		public static implicit operator double (Number num)
		{
			return (double)num.boxed;
		}
	}

}
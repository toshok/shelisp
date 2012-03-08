namespace Shelisp {
	public class Number : Object {
		public Number (object boxed)
		{
			this.boxed = boxed;
		}

		public override bool LispEq (Shelisp.Object other)
		{
			if (!(other is Number))
				return false;
			// lisp numbers are eq if their values are equal
			return ((Shelisp.Number)other).boxed == boxed;
		}

		public override bool LispEqual (Shelisp.Object other)
		{
			if (!(other is Number))
				return false;
			// lisp numbers are equal if their values are equal
			return ((Shelisp.Number)other).boxed == boxed;
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

		[LispBuiltin ("+", MinArgs = 2)]
		public static Shelisp.Object Fadd (L l, Shelisp.Object op1, Shelisp.Object op2)
		{
			if (!(op1 is Number)) throw new WrongTypeArgumentException ("numberp", op1);
			if (!(op2 is Number)) throw new WrongTypeArgumentException ("numberp", op2);

			Number ln = (Number)op1;
			Number rn = (Number)op2;

			bool need_float = (ln.boxed is float || rn.boxed is float);

			if (need_float) {
				return new Number ((float)ln.boxed + (float)rn.boxed);
			}
			else
				return new Number ((int)ln.boxed + (int)rn.boxed);
		}

		[LispBuiltin ("-", MinArgs = 2)]
		public static Shelisp.Object Fsub (L l, Shelisp.Object op1, Shelisp.Object op2)
		{
			if (!(op1 is Number)) throw new WrongTypeArgumentException ("numberp", op1);
			if (!(op2 is Number)) throw new WrongTypeArgumentException ("numberp", op2);

			Number ln = (Number)op1;
			Number rn = (Number)op2;

			bool need_float = (ln.boxed is float || rn.boxed is float);

			if (need_float) {
				return new Number ((float)ln.boxed - (float)rn.boxed);
			}
			else
				return new Number ((int)ln.boxed - (int)rn.boxed);
		}

		[LispBuiltin ("/", MinArgs = 2)]
		public static Shelisp.Object Fdiv (L l, Shelisp.Object op1, Shelisp.Object op2)
		{
			if (!(op1 is Number)) throw new WrongTypeArgumentException ("numberp", op1);
			if (!(op2 is Number)) throw new WrongTypeArgumentException ("numberp", op2);

			Number ln = (Number)op1;
			Number rn = (Number)op2;

			bool need_float = (ln.boxed is float || rn.boxed is float);

			if (need_float) {
				return new Number ((float)ln.boxed / (float)rn.boxed);
			}
			else
				return new Number ((int)ln.boxed / (int)rn.boxed);
		}

		[LispBuiltin ("*", MinArgs = 2)]
		public static Shelisp.Object Fmul (L l, Shelisp.Object op1, Shelisp.Object op2)
		{
			if (!(op1 is Number)) throw new WrongTypeArgumentException ("numberp", op1);
			if (!(op2 is Number)) throw new WrongTypeArgumentException ("numberp", op2);

			Number ln = (Number)op1;
			Number rn = (Number)op2;

			bool need_float = (ln.boxed is float || rn.boxed is float);

			if (need_float) {
				return new Number ((float)ln.boxed * (float)rn.boxed);
			}
			else
				return new Number ((int)ln.boxed * (int)rn.boxed);
		}
	
	}

}
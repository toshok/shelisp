using System;

namespace Shelisp {
	public class Number : Object {
		public Number (object boxed)
		{
			this.boxed = boxed;
		}

		public override int GetHashCode ()
		{
			if (boxed is int)
				return ((int)boxed).GetHashCode();
			else
				return ((float)boxed).GetHashCode();
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

		[LispBuiltin ("numberp", MinArgs = 1)]
		public static Shelisp.Object Fnumberp (L l, Shelisp.Object num)
		{
			return (num is Number) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("integerp", MinArgs = 1)]
		public static Shelisp.Object Fintegerp (L l, Shelisp.Object num)
		{
			return (num is Number && ((Number)num).boxed is int) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("floatp", MinArgs = 1)]
		public static Shelisp.Object Ffloatp (L l, Shelisp.Object num)
		{
			return (num is Number && ((Number)num).boxed is float) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("<", MinArgs = 2)]
		public static Shelisp.Object Fnumlt (L l, Shelisp.Object op1, Shelisp.Object op2)
		{
			if (!(op1 is Number)) throw new WrongTypeArgumentException ("numberp", op1);
			if (!(op2 is Number)) throw new WrongTypeArgumentException ("numberp", op2);

			Number ln = (Number)op1;
			Number rn = (Number)op2;

			bool need_float = (ln.boxed is float || rn.boxed is float);

			if (need_float) {
				return ((float)ln.boxed < (float)rn.boxed) ? L.Qt : L.Qnil;
			}
			else
				return ((int)ln.boxed < (int)rn.boxed) ? L.Qt : L.Qnil;
		}

		[LispBuiltin (">", MinArgs = 2)]
		public static Shelisp.Object Fnumgt (L l, Shelisp.Object op1, Shelisp.Object op2)
		{
			if (!(op1 is Number)) throw new WrongTypeArgumentException ("numberp", op1);
			if (!(op2 is Number)) throw new WrongTypeArgumentException ("numberp", op2);

			Number ln = (Number)op1;
			Number rn = (Number)op2;

			bool need_float = (ln.boxed is float || rn.boxed is float);

			if (need_float) {
				return ((float)ln.boxed > (float)rn.boxed) ? L.Qt : L.Qnil;
			}
			else
				return ((int)ln.boxed > (int)rn.boxed) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("=", MinArgs = 2)]
		public static Shelisp.Object Fnumequal (L l, Shelisp.Object op1, Shelisp.Object op2)
		{
			if (!(op1 is Number)) throw new WrongTypeArgumentException ("numberp", op1);
			if (!(op2 is Number)) throw new WrongTypeArgumentException ("numberp", op2);

			Number ln = (Number)op1;
			Number rn = (Number)op2;

			bool need_float = (ln.boxed is float || rn.boxed is float);

			if (need_float) {
				return ((float)ln.boxed == (float)rn.boxed) ? L.Qt : L.Qnil;
			}
			else
				return ((int)ln.boxed == (int)rn.boxed) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("eql", MinArgs = 2)]
		public static Shelisp.Object Feql (L l, Shelisp.Object op1, Shelisp.Object op2)
		{
			if (!(op1 is Number)) throw new WrongTypeArgumentException ("numberp", op1);
			if (!(op2 is Number)) throw new WrongTypeArgumentException ("numberp", op2);

			Number ln = (Number)op1;
			Number rn = (Number)op2;

			bool types_match = (ln.boxed is float) == (rn.boxed is float);
			if (!types_match)
				return L.Qnil;

			bool need_float = ln.boxed is float;

			if (need_float) {
				return ((float)ln.boxed == (float)rn.boxed) ? L.Qt : L.Qnil;
			}
			else
				return ((int)ln.boxed == (int)rn.boxed) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("+", MinArgs = 2)]
		public static Shelisp.Object Fadd (L l, Shelisp.Object op1, Shelisp.Object op2)
		{
			if (!(op1 is Number)) throw new WrongTypeArgumentException ("numberp", op1);
			if (!(op2 is Number)) throw new WrongTypeArgumentException ("numberp", op2);

			Number ln = (Number)op1;
			Number rn = (Number)op2;

			if (ln.boxed is float && rn.boxed is float) {
				return new Number ((float)ln.boxed + (float)rn.boxed);
			}
			else if (ln.boxed is float) {
				return new Number ((float)ln.boxed + (int)rn.boxed);
			}
			else if (rn.boxed is float) {
				return new Number ((int)ln.boxed + (float)rn.boxed);
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

			if (ln.boxed is float && rn.boxed is float) {
				return new Number ((float)ln.boxed - (float)rn.boxed);
			}
			else if (ln.boxed is float) {
				return new Number ((float)ln.boxed - (int)rn.boxed);
			}
			else if (rn.boxed is float) {
				return new Number ((int)ln.boxed - (float)rn.boxed);
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

			if (ln.boxed is float && rn.boxed is float) {
				return new Number ((float)ln.boxed / (float)rn.boxed);
			}
			else if (ln.boxed is float) {
				return new Number ((float)ln.boxed / (int)rn.boxed);
			}
			else if (rn.boxed is float) {
				return new Number ((int)ln.boxed / (float)rn.boxed);
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

			if (ln.boxed is float && rn.boxed is float) {
				return new Number ((float)ln.boxed * (float)rn.boxed);
			}
			else if (ln.boxed is float) {
				return new Number ((float)ln.boxed * (int)rn.boxed);
			}
			else if (rn.boxed is float) {
				return new Number ((int)ln.boxed * (float)rn.boxed);
			}
			else
				return new Number ((int)ln.boxed * (int)rn.boxed);
		}

		[LispBuiltin ("1+", MinArgs = 1)]
		public static Shelisp.Object Finc (L l, Shelisp.Object op)
		{
			if (!(op is Number)) throw new WrongTypeArgumentException ("numberp", op);

			Number ln = (Number)op;

			bool need_float = ln.boxed is float;

			if (need_float) {
				return new Number ((float)ln.boxed + 1);
			}
			else
				return new Number ((int)ln.boxed + 1);
		}

		[LispBuiltin ("1-", MinArgs = 1)]
		public static Shelisp.Object Fdec (L l, Shelisp.Object op)
		{
			if (!(op is Number)) throw new WrongTypeArgumentException ("numberp", op);

			Number ln = (Number)op;

			bool need_float = ln.boxed is float;

			if (need_float) {
				return new Number ((float)ln.boxed - 1);
			}
			else
				return new Number ((int)ln.boxed - 1);
		}
	
		[LispBuiltin ("logior", MinArgs = 0)]
		public static Shelisp.Object Flogior (L l, params Shelisp.Object[] ints_or_markers)
		{
			int result = 0;
			foreach (var obj in ints_or_markers) {
				if (obj is Number && ((Number)obj).boxed is int) {
					result |= (int)((Number)obj).boxed;
				}
#if EDITOR
				else if (obj is Shelisp.Editor.Marker) {
					result |= ((Marker)obj).Position;
				}
#endif
				else {
					throw new WrongTypeArgumentException ("int-or-marker-p", obj);
				}
			}
			return new Number (result);
		}
	}

}
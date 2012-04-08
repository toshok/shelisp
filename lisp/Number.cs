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
			return this.LispEql (other);
		}

		public override bool LispEqual (Shelisp.Object other)
		{
			return this.LispEql (other);
		}

		public override bool LispEql (Shelisp.Object other)
		{
			if (!(other is Number))
				return false;

			if (Number.IsInt (this) != Number.IsInt (other))
				return false;

			if (Number.IsInt (this)) {
				return Number.ToInt (this) == Number.ToInt (other);
			}
			else {
				return Number.ToFloat (this) == Number.ToFloat (other);
			}
		}

		public object boxed;

		public static bool IsInt (Shelisp.Object obj)
		{
			return (obj is Number) && ((Number)obj).boxed is int;
		}

		public static bool IsFloat (Shelisp.Object obj)
		{
			return (obj is Number) && ((Number)obj).boxed is float;
		}

		public static int ToInt (Shelisp.Object obj)
		{
			if (((Number)obj).boxed is int)
				return (int)((Number)obj).boxed;
			else
				return (int)(float)((Number)obj).boxed;
		}

		public static float ToFloat (Shelisp.Object obj)
		{
			if (((Number)obj).boxed is float)
				return (float)((Number)obj).boxed;
			else
				return (float)(int)((Number)obj).boxed;
		}

		public override string ToString(string format_type)
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

		[LispBuiltin]
		public static Shelisp.Object Fnumberp (L l, Shelisp.Object num)
		{
			return (num is Number) ? L.Qt : L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fintegerp (L l, Shelisp.Object num)
		{
			return (num is Number && ((Number)num).boxed is int) ? L.Qt : L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Ffloatp (L l, Shelisp.Object num)
		{
			return (num is Number && ((Number)num).boxed is float) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("<")]
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

		[LispBuiltin ("<=")]
		public static Shelisp.Object Fnumle (L l, Shelisp.Object op1, Shelisp.Object op2)
		{
			if (!(op1 is Number)) throw new WrongTypeArgumentException ("numberp", op1);
			if (!(op2 is Number)) throw new WrongTypeArgumentException ("numberp", op2);

			Number ln = (Number)op1;
			Number rn = (Number)op2;

			bool need_float = (ln.boxed is float || rn.boxed is float);

			if (need_float) {
				return ((float)ln.boxed <= (float)rn.boxed) ? L.Qt : L.Qnil;
			}
			else
				return ((int)ln.boxed <= (int)rn.boxed) ? L.Qt : L.Qnil;
		}

		[LispBuiltin (">")]
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

		[LispBuiltin (">=")]
		public static Shelisp.Object Fnumge (L l, Shelisp.Object op1, Shelisp.Object op2)
		{
			if (!(op1 is Number)) throw new WrongTypeArgumentException ("numberp", op1);
			if (!(op2 is Number)) throw new WrongTypeArgumentException ("numberp", op2);

			Number ln = (Number)op1;
			Number rn = (Number)op2;

			bool need_float = (ln.boxed is float || rn.boxed is float);

			if (need_float) {
				return ((float)ln.boxed >= (float)rn.boxed) ? L.Qt : L.Qnil;
			}
			else
				return ((int)ln.boxed >= (int)rn.boxed) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("=")]
		public static Shelisp.Object Fnumequal (L l, Shelisp.Object op1, Shelisp.Object op2)
		{
			if (!(op1 is Number)) throw new WrongTypeArgumentException ("numberp", op1);
			if (!(op2 is Number)) throw new WrongTypeArgumentException ("numberp", op2);

			Number ln = (Number)op1;
			Number rn = (Number)op2;

			bool need_float = Number.IsFloat(ln) || Number.IsFloat (rn);

			if (need_float)
				return Number.ToFloat(ln) == Number.ToFloat(rn) ? L.Qt : L.Qnil;
			else
				return Number.ToInt(ln) == Number.ToInt(rn) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("/=")]
		public static Shelisp.Object Fnumnotequal (L l, Shelisp.Object op1, Shelisp.Object op2)
		{
			return Control.Fnot (l, Fnumequal (l, op1, op2));
		}

		[LispBuiltin ("+")]
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

		[LispBuiltin ("-")]
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
		public static Shelisp.Object Fdiv (L l, [LispRest] params Shelisp.Object[] operands)
		{
			bool use_float = false;

			foreach (var operand in operands) {
				if (!(operand is Number)) throw new WrongTypeArgumentException ("numberp", operand);

				if (Number.IsFloat (operand))
					use_float = true;
			}

			if (use_float) {
				float result = Number.ToFloat (operands[0]);
				for (int i = 1; i < operands.Length; i ++)
					result /= Number.ToFloat (operands[i]);
				return new Number (result);
			}
			else {
				float result = Number.ToInt (operands[0]);
				for (int i = 1; i < operands.Length; i ++) {
					result /= Number.ToInt (operands[i]);
					if (result == 0)
						break;
				}
				return new Number (result);
			}
		}

		[LispBuiltin ("*")]
		public static Shelisp.Object Fmul (L l, [LispRest] params Shelisp.Object[] operands)
		{
			bool use_float = false;

			foreach (var operand in operands) {
				if (!(operand is Number)) throw new WrongTypeArgumentException ("numberp", operand);

				if (Number.IsFloat (operand))
					use_float = true;
			}

			if (use_float) {
				float result = 1.0f;
				foreach (var operand in operands)
					result *= Number.ToFloat (operand);
				return new Number (result);
			}
			else {
				int result = 1;
				foreach (var operand in operands)
					result *= Number.ToInt (operand);
				return new Number (result);
			}
		}

		[LispBuiltin ("1+")]
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

		[LispBuiltin ("1-")]
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
	
		[LispBuiltin]
		public static Shelisp.Object Flogior (L l, [LispRest] params Shelisp.Object[] ints_or_markers)
		{
			int result = 0;
			foreach (var obj in ints_or_markers) {
				if (Number.IsInt(obj)) {
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

		[LispBuiltin]
		public static Shelisp.Object Flogand (L l, params Shelisp.Object[] ints_or_markers)
		{
			int result = 0;
			foreach (var obj in ints_or_markers) {
				if (Number.IsInt(obj)) {
					result &= (int)((Number)obj).boxed;
				}
#if EDITOR
				else if (obj is Shelisp.Editor.Marker) {
					result &= ((Marker)obj).Position;
				}
#endif
				else {
					throw new WrongTypeArgumentException ("int-or-marker-p", obj);
				}
			}
			return new Number (result);
		}

		[LispBuiltin]
		public static Shelisp.Object Flsh (L l, Shelisp.Object integer1, Shelisp.Object count)
		{
			if (!Number.IsInt(integer1))
				throw new WrongTypeArgumentException ("integerp", integer1);

			if (!Number.IsInt(count))
				throw new WrongTypeArgumentException ("integerp", count);

			int integer1_i = Number.ToInt (integer1);
			int count_i = Number.ToInt (count);

			return new Number (integer1_i << count_i);
		}

		[LispBuiltin (MinArgs = 1)]
		public static Shelisp.Object Fmin (L l, [LispRest] params Shelisp.Object[] ints_or_markers)
		{
			int min = Int32.MaxValue;

			foreach (var obj in ints_or_markers) {
				if (Number.IsInt(obj)) {
					int i = Number.ToInt (obj);
					if (i < min)
						min = i;
				}
#if EDITOR
				else if (obj is Shelisp.Editor.Marker) {
					int i = ((Marker)obj).Position;
					if (i < min)
						min = i;
				}
#endif
				else {
					throw new WrongTypeArgumentException ("int-or-marker-p", obj);
				}
			}
			return new Number (min);
		}

		[LispBuiltin (MinArgs = 1)]
		public static Shelisp.Object Fmax (L l, [LispRest] params Shelisp.Object[] ints_or_markers)
		{
			int max = Int32.MinValue;

			foreach (var obj in ints_or_markers) {
				if (Number.IsInt(obj)) {
					int i = Number.ToInt (obj);
					if (i > max)
						max = i;
				}
#if EDITOR
				else if (obj is Shelisp.Editor.Marker) {
					int i = ((Marker)obj).Position;
					if (i > max)
						max = i;
				}
#endif
				else {
					throw new WrongTypeArgumentException ("int-or-marker-p", obj);
				}
			}
			return new Number (max);
		}
	}

}
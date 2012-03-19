using System;
namespace Shelisp {
	public class Object {
		public Object ()
		{
		}

		public Object (string description)
		{
			this.description = description;
		}

		public override string ToString ()
		{
			return ToString ("prin1");
		}

		public virtual string ToString(string format_type)
		{
			switch (format_type) {
			case "prin1":
			case "princ":
				return description ?? base.ToString();
			default:
				throw new NotSupportedException (string.Format ("unsupported format type `{0}'", format_type));
			}
		}

		private string description;

		public virtual Shelisp.Object Eval (L l, Shelisp.Object env = null)
		{
			// default implementation is for self-evaluating forms
			return this;
		}

		public virtual bool LispEq (Shelisp.Object other)
		{
			// default implementation only tests for reference equality
			return object.ReferenceEquals (this, other);
		}

		public virtual bool LispEql (Shelisp.Object other)
		{
			// default implementation only tests for reference equality
			return object.ReferenceEquals (this, other);
		}

		public virtual bool LispEqual (Shelisp.Object other)
		{
			// default implementation only tests for reference equality
			return object.ReferenceEquals (this, other);
		}

		public static implicit operator Object (byte num)
		{
			return new Number (num);
		}

		public static implicit operator Object (char num)
		{
			return new Number (num);
		}

		public static implicit operator Object (short num)
		{
			return new Number (num);
		}

		public static implicit operator Object (int num)
		{
			return new Number (num);
		}

		public static implicit operator Object (long num)
		{
			return new Number (num);
		}

		public static implicit operator Object (float num)
		{
			return new Number (num);
		}

		public static implicit operator Object (double num)
		{
			return new Number (num);
		}

		public static implicit operator Object (System.String str)
		{
			return new String(str);
		}

		[LispBuiltin]
		public static Shelisp.Object Feq (L l, Shelisp.Object o1, Shelisp.Object o2)
		{
			if (L.NILP(o1))
				return L.NILP(o2) ? L.Qt : L.Qnil;

			return o1.LispEq (o2) ? L.Qt : L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Feql (L l, Shelisp.Object o1, Shelisp.Object o2)
		{
			if (L.NILP(o1))
				return L.NILP(o2) ? L.Qt : L.Qnil;

			return o1.LispEql (o2) ? L.Qt : L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fequal (L l, Shelisp.Object o1, Shelisp.Object o2)
		{
			if (L.NILP(o1))
				return L.NILP(o2) ? L.Qt : L.Qnil;

			return o1.LispEqual (o2) ? L.Qt : L.Qnil;
		}
	}

}
using System;

namespace Shelisp {

	public class WrongTypeArgumentException : ArgumentException
	{
		public WrongTypeArgumentException (string predicate, Shelisp.Object value)
			: base (string.Format ("(wrong-type-argument {0} {1}/{2})", predicate, value, value.GetType()))
		{
		}
	}

}
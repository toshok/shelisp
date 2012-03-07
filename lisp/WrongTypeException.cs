using System;

namespace Lisp {

	public class WrongTypeArgumentException : ArgumentException
	{
		public WrongTypeArgumentException (string predicate, Lisp.Object value)
			: base (string.Format ("(wrong-type-argument {0} {1})", predicate, value))
		{
		}
	}

}
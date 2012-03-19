using System;

namespace Shelisp {

	[AttributeUsage(AttributeTargets.Parameter, Inherited = true)]
	public class LispRestAttribute : Attribute {
		public LispRestAttribute ()
		{
		}
	}

}
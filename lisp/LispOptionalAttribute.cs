using System;

namespace Shelisp {

	[AttributeUsage(AttributeTargets.Parameter, Inherited = true)]
	public class LispOptionalAttribute : Attribute {
		public LispOptionalAttribute ()
		{
		}
	}

}
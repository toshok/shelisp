using System;

namespace Shelisp {

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
	public class LispBuiltinAttribute : Attribute {
		public LispBuiltinAttribute ()
		{
			MinArgs = -1;
		}

		public LispBuiltinAttribute (string name)
		{
			Name = name;
			MinArgs = -1;
		}

		public string Name { get; set; }
		public int MinArgs { get; set; }
		public string DocString { get; set; }
		public bool Unevalled { get; set; } // true if the arguments should not be eval'ed before calling the function
	}

}
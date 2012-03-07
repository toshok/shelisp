using System;
using System.Diagnostics;

namespace Lisp {
	class Debug {
		[Conditional("DEBUG")]
		public static void Print (object o)
		{
			Console.WriteLine (o.ToString());
		}

		[Conditional("DEBUG")]
		public static void Print (string format, params object[] args)
		{
			Console.WriteLine (string.Format (format, args));
		}
	}

}
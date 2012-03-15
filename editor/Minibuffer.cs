using System;
using System.IO;
using Shelisp;

namespace Shemacs.Editor {

	public class Minibuffer {

		[LispBuiltin ("completing-read", MinArgs = 2)]
		public static Shelisp.Object Fcompleting_read (L l, Shelisp.Object prompt, Shelisp.Object collection, Shelisp.Object predicate, Shelisp.Object require_match,
							       Shelisp.Object initial_input, Shelisp.Object hist, Shelisp.Object def, Shelisp.Object inherit_input_method)
		{
			Console.Write (prompt);
			Console.Write (":");
			return (Shelisp.String)Console.ReadLine ();
		}
	}

}

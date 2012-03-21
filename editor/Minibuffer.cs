using System;
using System.IO;
using Shelisp;

namespace Shemacs.Editor {

	public class Minibuffer {

		[LispBuiltin ("completing-read", MinArgs = 2)]
		public static Shelisp.Object Fcompleting_read (L l, Shelisp.Object prompt, Shelisp.Object collection, Shelisp.Object predicate, Shelisp.Object require_match,
							       Shelisp.Object initial_input, Shelisp.Object hist, Shelisp.Object def, Shelisp.Object inherit_input_method)
		{
			Console.Write (prompt.ToString("princ"));
			return (Shelisp.String)Console.ReadLine ();
		}

		[LispBuiltin]
		public static Shelisp.Object Fall_completions (L l, Shelisp.Object str, Shelisp.Object collection, Shelisp.Object predicate, Shelisp.Object hide_spaces)
		{
			Console.WriteLine ("all-completions not implemented");
			return L.Qnil;
		}

		[LispBuiltin (DocString = @"Text properties that are added to minibuffer prompts.
These are in addition to the basic `field' property, and stickiness
properties.")]
		public static Shelisp.Object Vminibuffer_prompt_properties = new List (L.intern ("read-only"), new List (L.Qt, L.Qnil));
	}

}

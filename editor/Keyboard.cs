using System;
using Shelisp;

namespace Shemacs.Editor {

	public class Keyboard {

		[LispBuiltin]
		public static Shelisp.Object last_input_event = L.Qnil;

		[LispBuiltin]
		public static Shelisp.Object last_command_event = L.Qnil;

		[LispBuiltin (DocString = @"Character to recognize as meaning Help.
When it is read, do `(eval help-form)', and display result if it's a string.
If the value of `help-form' is nil, this char can be read normally.")]
		public Shelisp.Object Vhelp_char = new Number (Ctl ('H'));

		public static int Ctl (int key)
		{
			return key - '@';
		}
	}

}
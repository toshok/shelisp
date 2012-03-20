using System;
using Shelisp;

namespace Shemacs.Editor {

	public class Keyboard {

		[LispBuiltin]
		public static Shelisp.Object last_input_event = L.Qnil;

		[LispBuiltin]
		public static Shelisp.Object last_command_event = L.Qnil;
	}

}
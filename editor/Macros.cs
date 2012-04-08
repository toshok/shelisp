using System;
using Shelisp;

namespace Shemacs.Editor {
	public class Macros {

		[LispBuiltin]
		public static Shelisp.Object Vexecuting_kbd_macro = L.Qnil;

		[LispBuiltin]
		public static Shelisp.Object Vexecuting_macro = L.Qnil;

		[LispBuiltin (DocString = @"Record subsequent keyboard input, defining a keyboard macro.
The commands are recorded even as they are executed.
Use \\[end-kbd-macro] to finish recording and make the macro available.
Use \\[name-last-kbd-macro] to give it a permanent name.
Non-nil arg (prefix arg) means append to last macro defined;
this begins by re-executing that macro as if you typed it again.
If optional second arg, NO-EXEC, is non-nil, do not re-execute last
macro before appending to it.")]
		public static Shelisp.Object Fstart_kbd_macro (L l, Shelisp.Object append, [LispOptional] Shelisp.Object no_exec)
		{
			Console.WriteLine ("start-kbd-macro not implemented"); return L.Qnil;
		}
	}
}
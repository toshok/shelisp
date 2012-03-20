
using Shelisp;

namespace Shemacs.Editor {
	public class Macros {

		[LispBuiltin]
		public static Shelisp.Object Vexecuting_kbd_macro = L.Qnil;

		[LispBuiltin]
		public static Shelisp.Object Vexecuting_macro = L.Qnil;

	}
}
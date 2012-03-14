using Shelisp;

namespace Shelisp.Editor {

	public class Keymap : Object {
		[LispBuiltin ("define-key", MinArgs = 3)]
		public static Shelisp.Object Fdefine_key (L l, Shelisp.Object keymap, Shelisp.Object key, Shelisp.Object binding)
		{
			// XXX more here
			return binding;
		}

		[LispBuiltin ("make-sparse-keymap", MinArgs = 0)]
		public static Shelisp.Object Fdefine_key (L l, Shelisp.Object prompt)
		{
			return new List (L.intern ("keymap"), L.Qnil);
		}
	}
}
using Shelisp;

namespace Shemacs.Editor {
	class Fringe {

		[LispBuiltin (DocString = @"*Non-nil means that newline may flow into the right fringe.
This means that display lines which are exactly as wide as the window
(not counting the final newline) will only occupy one screen line, by
showing (or hiding) the final newline in the right fringe; when point
is at the final newline, the cursor is shown in the right fringe.
If nil, also continue lines which are exactly as wide as the window.")]
		public static bool overflow_newline_into_fringe = true;

		[LispBuiltin (DocString= @"List of fringe bitmap symbols.")]
		public static Shelisp.Object Vfringe_bitmaps = L.Qnil;

	}
}
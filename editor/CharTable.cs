using Shelisp;

namespace Shelisp.Editor {

	public class CharTable : Object {
		[LispBuiltin ("make-char-table", MinArgs = 1)]
		public static Shelisp.Object Fmake_char_table (L l, Shelisp.Object subtype, Shelisp.Object init)
		{
			return new CharTable();
		}
	}
}
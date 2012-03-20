
using Shelisp;

namespace Shemacs.Editor {

	class CaseTable {

		[LispBuiltin (DocString = @"Return the standard case table.
			       This is the one used for new buffers.")]
		public static Shelisp.Object standard_case_table (L l)
		{
			return Vascii_downcase_table;
		}

		[LispBuiltin]
		public static Shelisp.Object Vascii_downcase_table = L.Qnil;
	}
}
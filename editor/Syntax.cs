using Shelisp;

namespace Shemacs.Editor {

	public class Syntax : Object {

		[LispBuiltin (DocString = @"Return the standard syntax table.
This is the one used for new buffers.")]
		public static Shelisp.Object Fstandard_syntax_table (L l)
		{
			return Vstandard_syntax_table;
		}

		//[LispBuiltin]
		public static Shelisp.Object Vstandard_syntax_table = new CharTable (/*XXX Qsyntax_table, temp*/);
	}

}


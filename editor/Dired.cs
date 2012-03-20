using Shelisp;

namespace Shemacs.Editor {

	public class Dired {

		[LispBuiltin (DocString = @"Completion ignores file names ending in any string in this list.
It does not ignore them if all possible completions end in one of
these strings or when displaying a list of completions.
It ignores directory names if they match any string in this list which
ends in a slash")]
		public static Shelisp.Object Vcompletion_ignored_extensions = L.Qnil;
	}

}
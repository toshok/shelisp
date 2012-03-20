
using Shelisp;

namespace Shemacs.Editor {
	public static class Purity {

		[LispBuiltin]
		public static Shelisp.Object Fpurecopy (L l, Shelisp.Object obj)
		{
			return obj;
		}
	}
}
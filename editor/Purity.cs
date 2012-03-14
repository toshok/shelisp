
using Shelisp;

namespace Shelisp.Editor {
	public static class Purity {

		[LispBuiltin ("purecopy", MinArgs = 1)]
		public static Shelisp.Object Fpurecopy (L l, Shelisp.Object obj)
		{
			return obj;
		}
	}
}
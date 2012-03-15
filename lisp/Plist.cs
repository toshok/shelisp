using Shelisp;

namespace Shelisp.Editor {

	public class Plist : Object {
		[LispBuiltin ("plist-put", MinArgs = 3)]
		public static Shelisp.Object Fplist_put (L l, Shelisp.Object plist, Shelisp.Object property, Shelisp.Object value)
		{
			return value;
		}

		[LispBuiltin ("plist_get", MinArgs = 2)]
		public static Shelisp.Object Fplist_get (L l, Shelisp.Object plist, Shelisp.Object property)
		{
			return plist;
		}
	}
}

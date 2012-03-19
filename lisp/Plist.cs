using System;
using Shelisp;

namespace Shelisp {

	public class Plist : Object {
		[LispBuiltin]
		public static Shelisp.Object Fplist_put (L l, Shelisp.Object plist, Shelisp.Object property, Shelisp.Object value)
		{
			// XXX let's be assholes and not replace anything in the list.  just add onto the front like a boss.
			return new List (property, new List (value, plist));
		}

		[LispBuiltin]
		public static Shelisp.Object Fplist_get (L l, Shelisp.Object plist, Shelisp.Object property)
		{
			Shelisp.Object el = plist;
			Shelisp.Object val = L.Qnil;

			while (!L.NILP (el)) {
				if (L.CAR(el).LispEq (property)) {
					// the next thing in the list is the value
					val = L.CAR(L.CDR(el));
					break;
				}

				el = L.CDR (el);
			}

			return val;
		}
	}
}

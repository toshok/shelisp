using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shelisp {
	public static class Obarray {
		private static int oblookup_last_bucket_number;
		private static Shelisp.Object oblookup (Vector obarray, string str)
		{
#if not_ported
			if (!VECTORP (obarray)
			    || (obsize = ASIZE (obarray)) == 0) {
				obarray = check_obarray (obarray);
				obsize = ASIZE (obarray);
			}
#endif

			int hash = (int)(((uint)str.GetHashCode()) % obarray.Length);

			Shelisp.Object bucket = obarray[hash];
			oblookup_last_bucket_number = hash;

			if (Number.IsInt (bucket) && Number.ToInt (bucket) == 0) {
				/* empty */;
			}
			else if (!(bucket is Symbol))
				throw new LispErrorException ("Bad data in guts of obarray"); /* Like CADR error message */
			else {
				Shelisp.Symbol tail = (Symbol)bucket;
				while (tail != null) {
					if (tail.name == str)
						return tail;
					tail = tail.next;
				}
			}
			return new Number (hash);
		}

		public static Shelisp.Symbol Intern (Vector obarray, string str)
		{
			var existing = oblookup (obarray, str);

			if (existing is Symbol)
				return (Shelisp.Symbol)existing;

			/* it doesn't exist in the obarray, we need to add it */
			Shelisp.Symbol sym = new Shelisp.Symbol (str);
			int bucket_num = Number.ToInt (existing);

			if (!(Number.IsInt (obarray[bucket_num])))
				sym.next = (Shelisp.Symbol)obarray[bucket_num];
			obarray[bucket_num] = sym;

			return sym;
		}

		public static Shelisp.Object InternSoft (Vector obarray, string str)
		{
			var existing = oblookup (obarray, str);

			if (existing is Symbol)
				return existing;

			return L.Qnil;
		}

		public static Shelisp.Object Unintern (Vector obarray, string str)
		{
			var existing = oblookup (obarray, str);
			if (existing is Symbol) {
				Symbol sym = (Symbol)existing;
				if (sym.name == str) {
					// the head of the bucket is what we were looking for, remove it.
					obarray[oblookup_last_bucket_number] = sym.next;
					sym.next = null;
					return L.Qt;
				}
				else {
					Shelisp.Symbol prev = sym;
					Shelisp.Symbol tail = sym.next;

					while (tail != null) {
						if (tail.name == str) {
							prev.next = tail.next;
							tail.next = null;
							return L.Qt;
						}
						prev = tail;
						tail = tail.next;
					}
				}
			}

			return L.Qnil;
		}
	}
}
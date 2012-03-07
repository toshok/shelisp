using System;
using System.Collections;
using System.Collections.Generic;

namespace Shelisp {
	public abstract class Sequence : Object, IEnumerable<Shelisp.Object> {
		public abstract int Length { get; }
		public abstract IEnumerator<Shelisp.Object> GetEnumerator ();
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}

		[LispBuiltin ("sequencep", MinArgs = 1)]
		public static Shelisp.Object Fsequencep(L l, Shelisp.Object o)
		{
			return (o is Sequence) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("elt", MinArgs = 2)]
		public static Shelisp.Object Felt(L l, Shelisp.Object seq, Shelisp.Object index)
		{
			// XXX add type checks
			int idx = (int)(Number)index;
			foreach (var el in (Sequence)seq)
				if (idx-- == 0)
					return el;
			return L.Qnil;
		}

		[LispBuiltin ("length", MinArgs = 1)]
		public static Shelisp.Object Flength(L l, Shelisp.Object o)
		{
			if (!(o is Sequence))
				throw new Exception ("non-seq passed to length");

			Sequence seq = (Sequence)o;
			return seq.Length;
		}

		[LispBuiltin ("copy-sequence", MinArgs = 1)]
		public static Shelisp.Object Fcopy_sequence(L l, Shelisp.Object seq)
		{
			// XXX shallow copy of the sequence seq, 1 level deep.
			throw new NotImplementedException ();
		}

		[LispBuiltin ("mapcar", MinArgs = 2)]
		public static Shelisp.Object Fmapcar(L l, Shelisp.Object fun, Shelisp.Object seq)
		{
			if (!(seq is Sequence))
				throw new Exception ("non-sequence passed to mapcar");
			if (!(fun is Subr))
				throw new Exception ("non-function passed to mapcar");

			Sequence s = (Sequence)seq;
			Subr subr = (Subr)fun;

			List<Shelisp.Object> mapped = new List<Shelisp.Object>();
			Shelisp.Object[] call_arg = new Shelisp.Object[1];
			foreach (var o in s) {
				call_arg[0] = o;
				mapped.Add(subr.Call (l, call_arg));
			}
			// XXX this does the wrong thing since make_list takes params[] now
			return L.make_list (mapped.ToArray());
		}

	}
}
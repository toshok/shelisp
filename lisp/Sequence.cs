using System;
using System.Collections;
using System.Collections.Generic;

namespace Lisp {
	public abstract class Sequence : Object, IEnumerable<Lisp.Object> {
		public abstract int Length { get; }
		public abstract IEnumerator<Lisp.Object> GetEnumerator ();
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}

		[LispBuiltin ("sequencep", MinArgs = 1)]
		public static Lisp.Object Fsequencep(L l, Lisp.Object o)
		{
			return (o is Sequence) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("elt", MinArgs = 2)]
		public static Lisp.Object Felt(L l, Lisp.Object seq, Lisp.Object index)
		{
			// XXX add type checks
			int idx = (int)(Number)index;
			foreach (var el in (Sequence)seq)
				if (idx-- == 0)
					return el;
			return L.Qnil;
		}

		[LispBuiltin ("length", MinArgs = 1)]
		public static Lisp.Object Flength(L l, Lisp.Object o)
		{
			if (!(o is Sequence))
				throw new Exception ("non-seq passed to length");

			Sequence seq = (Sequence)o;
			return seq.Length;
		}

		[LispBuiltin ("copy-sequence", MinArgs = 1)]
		public static Lisp.Object Fcopy_sequence(L l, Lisp.Object seq)
		{
			// XXX shallow copy of the sequence seq, 1 level deep.
			throw new NotImplementedException ();
		}

		[LispBuiltin ("mapcar", MinArgs = 2)]
		public static Lisp.Object Fmapcar(L l, Lisp.Object fun, Lisp.Object seq)
		{
			if (!(seq is Sequence))
				throw new Exception ("non-sequence passed to mapcar");
			if (!(fun is Subr))
				throw new Exception ("non-function passed to mapcar");

			Sequence s = (Sequence)seq;
			Subr subr = (Subr)fun;

			List<Lisp.Object> mapped = new List<Lisp.Object>();
			Lisp.Object[] call_arg = new Lisp.Object[1];
			foreach (var o in s) {
				call_arg[0] = o;
				mapped.Add(subr.Call (l, call_arg));
			}
			// XXX this does the wrong thing since make_list takes params[] now
			return L.make_list (mapped.ToArray());
		}

	}
}
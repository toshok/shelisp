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
			if (L.Qnil.LispEq (seq))
				return L.Qnil;
			// XXX add type checks
			int idx = (int)(Number)index;
			foreach (var el in (Sequence)seq)
				if (idx-- == 0)
					return el;
			return L.Qnil;
		}

		[LispBuiltin ("nth", MinArgs = 2)]
		public static Shelisp.Object Fnth(L l, Shelisp.Object index, Shelisp.Object seq)
		{
			return Sequence.Felt (l, seq, index);
		}

		[LispBuiltin ("length", MinArgs = 1)]
		public static Shelisp.Object Flength(L l, Shelisp.Object o)
		{
			if (L.NILP (o))
				return 0;

			if (!(o is Sequence))
				throw new Exception ("non-seq passed to length");

			Sequence seq = (Sequence)o;
			return seq.Length;
		}

		[LispBuiltin ("copy-sequence", MinArgs = 1)]
		public static Shelisp.Object Fcopy_sequence(L l, Shelisp.Object seq)
		{
			// XXX bad bad bad
			return seq;
		}

		[LispBuiltin ("mapcar", MinArgs = 2)]
		public static Shelisp.Object Fmapcar(L l, Shelisp.Object fun, Shelisp.Object seq)
		{
			if (L.Qnil.LispEq (seq))
				return L.Qnil;

			if (!(seq is Sequence))
				throw new WrongTypeArgumentException ("sequencep", seq);

			Sequence s = (Sequence)seq;

			List<Shelisp.Object> mapped = new List<Shelisp.Object>();
			foreach (var o in s)
				mapped.Add(L.make_list (fun, new List (L.Qquote, new List (o, L.Qnil))).Eval (l));

			return new List (mapped.ToArray());
		}

		[LispBuiltin ("mapc", MinArgs = 2)]
		public static Shelisp.Object Fmapc(L l, Shelisp.Object fun, Shelisp.Object seq)
		{
			if (L.Qnil.LispEq (seq))
				return L.Qnil;

			if (!(seq is Sequence))
				throw new WrongTypeArgumentException ("sequencep", seq);

			Sequence s = (Sequence)seq;

			foreach (var o in s)
				L.make_list (fun, new List (L.Qquote, new List (o, L.Qnil))).Eval (l);

			return seq;
		}

		[LispBuiltin ("append", MinArgs = 0)]
		public static Shelisp.Object Fappend (L l, params Shelisp.Object[] sequences)
		{
			if (sequences.Length == 0)
				return L.Qnil;

			Shelisp.Object tail = sequences[sequences.Length - 1];
			for (int i = sequences.Length - 2; i >= 0; i --) {
				if (sequences[i] is Array) {
					Array arr = (Array)sequences[i];
					for (int j = arr.Length - 1; j >= 0; j --)
						tail = new List (arr[j], tail);
				}
				else if (sequences[i] is List) {
					Shelisp.List reversed = (Shelisp.List)Shelisp.List.reverse (sequences[i]);
					foreach (var el in reversed)
						tail = new List(el, tail);
				}
			}

			return tail;
		}
	}
}
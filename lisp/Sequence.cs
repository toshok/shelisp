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

		public override bool LispEqual (Shelisp.Object other)
		{
			// is this right?  can lists be equal to vectors if the elements are the same?
			if (other.GetType() != this.GetType()) {
				return false;
			}

			IEnumerator<Shelisp.Object> this_enum = this.GetEnumerator();
			IEnumerator<Shelisp.Object> other_enum = ((Sequence)other).GetEnumerator();

			while (this_enum.MoveNext()) {
				if (!other_enum.MoveNext())
					return false;

				if (!this_enum.Current.LispEqual (other_enum.Current))
				    return false;
			}

			if (other_enum.MoveNext()) // there are more items left in other_enum
				return false;

			return true;
		}


		[LispBuiltin]
		public static Shelisp.Object Fsequencep(L l, Shelisp.Object o)
		{
			return (o is Sequence) ? L.Qt : L.Qnil;
		}

		[LispBuiltin]
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

		[LispBuiltin]
		public static Shelisp.Object Fnth(L l, Shelisp.Object index, Shelisp.Object seq)
		{
			return Sequence.Felt (l, seq, index);
		}

		[LispBuiltin]
		public static Shelisp.Object Flength(L l, Shelisp.Object o)
		{
			if (L.NILP (o))
				return 0;

			if (!(o is Sequence))
				throw new Exception ("non-seq passed to length");

			Sequence seq = (Sequence)o;
			return seq.Length;
		}

		[LispBuiltin]
		public static Shelisp.Object Fcopy_sequence(L l, Shelisp.Object seq)
		{
			// XXX bad bad bad
			return seq;
		}

		[LispBuiltin]
		public static Shelisp.Object Fmapcar(L l, Shelisp.Object fun, Shelisp.Object seq)
		{
			if (L.NILP(seq))
				return L.Qnil;

			if (!(seq is Sequence))
				throw new WrongTypeArgumentException ("sequencep", seq);

			Sequence s = (Sequence)seq;

			List<Shelisp.Object> mapped = new List<Shelisp.Object>();
			foreach (var o in s) {
				var application = L.make_list (fun, new List (L.Qquote, new List (o, L.Qnil)));
				mapped.Add(application.Eval (l));
			}

			return new List (mapped.ToArray());
		}

		[LispBuiltin]
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

		[LispBuiltin]
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

		[LispBuiltin (DocString = @"Delete by side effect any occurrences of ELT as a member of SEQ.
SEQ must be a list, a vector, or a string.
The modified SEQ is returned.  Comparison is done with `equal'.
If SEQ is not a list, or the first member of SEQ is ELT, deleting it
is not a side effect; it is simply using a different sequence.
Therefore, write `(setq foo (delete element foo))'
to be sure of changing the value of `foo'.")]
		public static Shelisp.Object Fdelete (L l, Shelisp.Object elt, Shelisp.Object seq)
		{
			if (seq is Vector) {
				Vector seqv = (Vector)seq;
				int i, n;

				for (i = n = 0; i < seqv.Length; ++i)
					if (!seqv[i].LispEqual (elt))
						++n;

				if (n != seqv.Length) {
					Shelisp.Object[] p = new Shelisp.Object[n];

					for (i = n = 0; i < seqv.Length; ++i)
						if (!seqv[i].LispEqual (elt))
							p[n++] = seqv[i];

					seqv.SetData (p);
				}
			}
			else if (seq is String) {
				throw new NotImplementedException ();
			}
			else {
				Shelisp.Object tail, prev;

				for (tail = seq, prev = L.Qnil; L.CONSP (tail); tail = L.CDR (tail)) {
					// XXX CHECK_LIST_CONS (tail, seq);

					if (elt.LispEqual (L.CAR(tail))) {
						if (L.NILP (prev))
							seq = L.CDR (tail);
						else
							List.Fsetcdr (l, prev, L.CDR (tail));
					}
					else
						prev = tail;
				}
			}
			return seq;
		}
	}
}
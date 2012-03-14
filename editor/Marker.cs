using System;
using Shelisp;

namespace Shemacs.Editor {
	class Marker : Shelisp.Object {
		public Marker ()
		{
		}

		public override string ToString ()
		{
			string buffer_name = (this.Buffer == null || L.Qnil.LispEq (this.Buffer)) ? "no buffer" : (Shelisp.String)Buffer.name;

			if (Position == null)
				return string.Format ("#<marker in {0}>", buffer_name);
			else 
				return string.Format ("#<marker at {0} in {1}>", Position.Value, buffer_name);
		}

		/* Set the position of MARKER, specifying both the
		   character position and the corresponding byte position.  */
		public static Shelisp.Object SetBoth (Shelisp.Object marker, Shelisp.Object buffer, int charpos, int bytepos)
		{
			throw new NotImplementedException ();
		}

		[LispBuiltin ("move-marker", MinArgs = 2)]
		[LispBuiltin ("set-marker", MinArgs = 2)]
		public static Shelisp.Object Fset_marker (L l, Shelisp.Object marker, Shelisp.Object position, Shelisp.Object buffer)
		{
			throw new NotImplementedException ();
		}

		[LispBuiltin ("markerp", MinArgs = 1)]
		public static Shelisp.Object Fmarkerp (L l, Shelisp.Object marker)
		{
			return (marker is Marker) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("integer-or-marker-p", MinArgs = 1)]
		public static Shelisp.Object Finteger_or_marker_p (L l, Shelisp.Object obj)
		{
			return ((obj is Marker) || (obj is Number && ((Number)obj).boxed is int)) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("number-or-marker-p", MinArgs = 1)]
		public static Shelisp.Object Fnumber_or_marker_p (L l, Shelisp.Object obj)
		{
			return ((obj is Marker) || (obj is Number)) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("make-marker", MinArgs = 0)]
		public static Shelisp.Object Fmake_marker (L l)
		{
			return new Marker ();
		}

		[LispBuiltin ("copy-marker", MinArgs = 1)]
		public static Shelisp.Object Fcopy_marker (L l, Shelisp.Object marker_in_integer, Shelisp.Object insertion_type)
		{
			throw new NotImplementedException ();
		}

		[LispBuiltin ("set-marker-insertion-type", MinArgs = 2)]
		public static Shelisp.Object Fset_marker_insertion_type (L l, Shelisp.Object marker, Shelisp.Object insertion_type)
		{
			if (!(marker is Marker))
				throw new WrongTypeArgumentException ("markerp", marker);

			Marker m = (Marker)marker;

			if (L.Qt.LispEq (insertion_type))
				m.Type = InsertionType.MarkerAdvancesOnTextInsertion;
			else if (L.Qnil.LispEq (insertion_type))
				m.Type = InsertionType.MarkerDoesNotAdvanceOnTextInsertion;
			else
				throw new Exception ();

			return L.Qnil;
		}

		public enum InsertionType {
			MarkerAdvancesOnTextInsertion,      // maps to t insertion type
			MarkerDoesNotAdvanceOnTextInsertion // maps to nil insertion type
		}

		public int? Position { get; set; }
		public InsertionType Type { get; set; }
		public Buffer Buffer { get; set; }
	}
}
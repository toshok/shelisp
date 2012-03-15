using Shelisp;

namespace Shelisp.Editor {

	public class CharTable : Object {
		[LispBuiltin ("make-char-table", MinArgs = 1)]
		public static Shelisp.Object Fmake_char_table (L l, Shelisp.Object subtype, Shelisp.Object init)
		{
			return new CharTable();
		}

		[LispBuiltin ("char-table-subtype", MinArgs = 1)]
		public static Shelisp.Object Fchar_table_subtype (L l, Shelisp.Object char_table)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("char-table-parent")]
		public static Shelisp.Object Fchar_table_parent (L l, Shelisp.Object char_table)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("set-char-table-parent", MinArgs = 2)]
		public static Shelisp.Object Fchar_table_parent (L l, Shelisp.Object char_table, Shelisp.Object parent)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("char-table-extra-slot", MinArgs = 2)]
		public static Shelisp.Object Fchar_table_extra_slot (L l, Shelisp.Object char_table, Shelisp.Object n)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("set-char-table-extra-slot", MinArgs = 3)]
		public static Shelisp.Object Fset_char_table_extra_slot (L l, Shelisp.Object char_table, Shelisp.Object n, Shelisp.Object value)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("char-table-range", MinArgs = 2)]
		public static Shelisp.Object Fchar_table_range (L l, Shelisp.Object char_table, Shelisp.Object range)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("set-char-table-range", MinArgs = 3)]
		public static Shelisp.Object Fset_char_table_range (L l, Shelisp.Object char_table, Shelisp.Object range, Shelisp.Object value)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("set-char-table-default", MinArgs = 3)]
		public static Shelisp.Object Fset_char_table_default (L l, Shelisp.Object char_table, Shelisp.Object range, Shelisp.Object value)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("optimize-char-table", MinArgs = 1)]
		public static Shelisp.Object Foptimize_char_table (L l, Shelisp.Object char_table, Shelisp.Object test)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("map-char-table", MinArgs = 2)]
		public static Shelisp.Object Fmap_char_table (L l, Shelisp.Object function, Shelisp.Object char_table)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("unicode-property-table-internal", MinArgs = 1)]
		public static Shelisp.Object Funicode_property_table_internal (L l, Shelisp.Object prop)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("get-unicode-property-internal", MinArgs = 2)]
		public static Shelisp.Object Fget_unicode_property_internal (L l, Shelisp.Object char_table, Shelisp.Object ch)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("put-unicode-property-internal", MinArgs = 3)]
		public static Shelisp.Object Fset_unicode_property_internal (L l, Shelisp.Object char_table, Shelisp.Object ch, Shelisp.Object value)
		{
			// XXX
			return L.Qnil;
		}
			       
	}
}
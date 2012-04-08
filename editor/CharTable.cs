using Shelisp;

namespace Shemacs.Editor {

	public class CharTable : Object {
		[LispBuiltin]
		public static Shelisp.Object Fmake_char_table (L l, Shelisp.Object subtype, [LispOptional] Shelisp.Object init)
		{
			return new CharTable();
		}

		[LispBuiltin]
		public static Shelisp.Object Fchar_table_subtype (L l, Shelisp.Object char_table)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fchar_table_parent (L l, Shelisp.Object char_table)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fset_char_table_parent (L l, Shelisp.Object char_table, Shelisp.Object parent)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fchar_table_extra_slot (L l, Shelisp.Object char_table, Shelisp.Object n)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fset_char_table_extra_slot (L l, Shelisp.Object char_table, Shelisp.Object n, Shelisp.Object value)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fchar_table_range (L l, Shelisp.Object char_table, Shelisp.Object range)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fset_char_table_range (L l, Shelisp.Object char_table, Shelisp.Object range, Shelisp.Object value)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fset_char_table_default (L l, Shelisp.Object char_table, Shelisp.Object range, Shelisp.Object value)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Foptimize_char_table (L l, Shelisp.Object char_table, [LispOptional] Shelisp.Object test)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fmap_char_table (L l, Shelisp.Object function, Shelisp.Object char_table)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Funicode_property_table_internal (L l, Shelisp.Object prop)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fget_unicode_property_internal (L l, Shelisp.Object char_table, Shelisp.Object ch)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fput_unicode_property_internal (L l, Shelisp.Object char_table, Shelisp.Object ch, Shelisp.Object value)
		{
			// XXX
			return L.Qnil;
		}
			       
	}
}
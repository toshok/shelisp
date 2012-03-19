using Shelisp;

namespace Shelisp.Editor {

	public class Charset : Object {
		[LispBuiltin]
		public static Shelisp.Object Fput_charset_property (L l, Shelisp.Object charset, Shelisp.Object propname, Shelisp.Object value)
		{
			return value;
		}

		[LispBuiltin]
		public static Shelisp.Object Fcharsetp (L l, Shelisp.Object obj)
		{
			return obj is Charset ? L.Qt : L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fmap_charset_chars (L l, Shelisp.Object function, Shelisp.Object charset, [LispOptional] Shelisp.Object arg, Shelisp.Object from_code, Shelisp.Object to_code)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fdefine_charset_internal (L l, params Shelisp.Object[] args)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fdefine_charset_alias (L l, Shelisp.Object charset, Shelisp.Object alias)
		{
			return charset;
		}

		[LispBuiltin]
		public static Shelisp.Object Fcharset_plist (L l, Shelisp.Object charset)
		{
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fset_charset_plist (L l, Shelisp.Object charset, Shelisp.Object plist)
		{
			return plist;
		}

		[LispBuiltin]
		public static Shelisp.Object Funify_charset (L l, Shelisp.Object charset, [LispOptional] Shelisp.Object unify_map, Shelisp.Object deunify)
		{
			return charset;
		}

		[LispBuiltin]
		public static Shelisp.Object Fget_unused_iso_final_char (L l, Shelisp.Object dimension, Shelisp.Object chars)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fdeclare_equiv_charset (L l, Shelisp.Object dimension, Shelisp.Object chars, Shelisp.Object final_char, Shelisp.Object charset)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Ffind_charset_region (L l, Shelisp.Object beg, Shelisp.Object end, [LispOptional] Shelisp.Object table)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Ffind_charset_string (L l, Shelisp.Object str, [LispOptional] Shelisp.Object table)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fdecode_char (L l, Shelisp.Object charset, Shelisp.Object code_point, [LispOptional] Shelisp.Object restriction)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fencode_char (L l, Shelisp.Object ch, Shelisp.Object charset, [LispOptional] Shelisp.Object restriction)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fmake_char (L l, Shelisp.Object charset, [LispOptional] Shelisp.Object code1, Shelisp.Object code2, Shelisp.Object code3, Shelisp.Object code4)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fsplit_char (L l, Shelisp.Object ch)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fchar_charset (L l, Shelisp.Object ch, [LispOptional] Shelisp.Object restriction)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fcharset_after (L l, [LispOptional] Shelisp.Object pos)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fiso_charset (L l, Shelisp.Object dimension, Shelisp.Object chars, Shelisp.Object final_char)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fclear_charset_maps (L l)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fclear_charset_maps (L l, [LispOptional] Shelisp.Object highestp)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin (MinArgs = 1)]
		public static Shelisp.Object Fset_charset_priority (L l, params Shelisp.Object[] charsets)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fcharset_id_internal (L l, [LispOptional] Shelisp.Object charset)
		{
			// XXX
			return L.Qnil;
		}


		[LispBuiltin (DocString = "*List of directories to search for charset map files.")]
		public static Shelisp.Object Vcharset_map_path = L.Qnil;
	}
}
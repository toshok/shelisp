using Shelisp;

namespace Shelisp.Editor {

	public class Character : Object {

		[LispBuiltin ("characterp", MinArgs = 1)]
		public static Shelisp.Object Fcharacterp (L l, Shelisp.Object obj, Shelisp.Object ignore)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("max-char", MinArgs = 0)]
		public static Shelisp.Object Fmax_char (L l)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("unibyte-char-to-multibyte", MinArgs = 1)]
		public static Shelisp.Object Funibyte_char_to_multibyte (L l, Shelisp.Object ch)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("multibyte-char-to-unibyte", MinArgs = 1)]
		public static Shelisp.Object Fmultibyte_char_to_uniibyte (L l, Shelisp.Object ch)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("char-width", MinArgs = 1)]
		public static Shelisp.Object Fchar_width (L l, Shelisp.Object ch)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("string-width", MinArgs = 1)]
		public static Shelisp.Object Fstring_width (L l, Shelisp.Object ch)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("string", MinArgs = 0)]
		public static Shelisp.Object Fstring (L l, params Shelisp.Object[] args)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("unibyte-string", MinArgs = 0)]
		public static Shelisp.Object Funibyte_string (L l, params Shelisp.Object[] args)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("char-resolve-modifiers", MinArgs = 0)]
		public static Shelisp.Object Fchar_resolve_modifier (L l, Shelisp.Object character)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("get-byte", MinArgs = 0)]
		public static Shelisp.Object Fget_byte (L l, Shelisp.Object position, Shelisp.Object str)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("char-to-string", MinArgs = 1)]
		public static Shelisp.Object Fget_byte (L l, Shelisp.Object ch)
		{
			// XXX
			return L.Qnil;
		}
	}
}
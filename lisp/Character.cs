using System.Text;

using Shelisp;

namespace Shelisp.Editor {

	public class Character : Object {

		[LispBuiltin]
		public static Shelisp.Object Fcharacterp (L l, Shelisp.Object obj, [LispOptional] Shelisp.Object ignore)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fmax_char (L l)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Funibyte_char_to_multibyte (L l, Shelisp.Object ch)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fmultibyte_char_to_unibyte (L l, Shelisp.Object ch)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fchar_width (L l, Shelisp.Object ch)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fstring_width (L l, Shelisp.Object ch)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fstring (L l, params Shelisp.Object[] args)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var o in args) {
				sb.Append((char)Number.ToInt(o));
			}
			return new Shelisp.String(sb.ToString());
		}

		[LispBuiltin]
		public static Shelisp.Object Funibyte_string (L l, params Shelisp.Object[] args)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fchar_resolve_modifier (L l, Shelisp.Object character)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fget_byte (L l, Shelisp.Object position, Shelisp.Object str)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fget_byte (L l, Shelisp.Object ch)
		{
			// XXX
			return L.Qnil;
		}
	}
}

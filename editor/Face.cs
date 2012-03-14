using System;
using Shelisp;

namespace Shemacs.Editor {
	class Face : Shelisp.Object {

		// elisp manual says this is a macro?
		[LispBuiltin ("defface", MinArgs = 3)]
		public static Shelisp.Object Fdefface (L l, Shelisp.Object face, Shelisp.Object spec, Shelisp.Object doc, params Shelisp.Object[] rest)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("set-face-attribute", MinArgs = 2)]
		public static Shelisp.Object Fset_face_attribute (L l, Shelisp.Object face, Shelisp.Object frame, params Shelisp.Object[] arguments)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("face-attribute-relative-p", MinArgs = 2)]
		public static Shelisp.Object Fface_attribute_relative_p (L l, Shelisp.Object attribute, Shelisp.Object value)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("face-all-attributes", MinArgs = 1)]
		public static Shelisp.Object Fface_all_attributes (L l, Shelisp.Object face, Shelisp.Object frame)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("merge-face-attribute", MinArgs = 3)]
		public static Shelisp.Object Fmerge_face_attribute (L l, Shelisp.Object attribute, Shelisp.Object value1, Shelisp.Object value2)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("set-face-foreground", MinArgs = 2)]
		public static Shelisp.Object Fset_face_foreground (L l, Shelisp.Object face, Shelisp.Object color, Shelisp.Object frame)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("set-face-background", MinArgs = 2)]
		public static Shelisp.Object Fset_face_background (L l, Shelisp.Object face, Shelisp.Object color, Shelisp.Object frame)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("set-face-stipple", MinArgs = 2)]
		public static Shelisp.Object Fset_face_stipple (L l, Shelisp.Object face, Shelisp.Object pattern, Shelisp.Object frame)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("set-face-font", MinArgs = 2)]
		public static Shelisp.Object Fset_face_font (L l, Shelisp.Object face, Shelisp.Object font, Shelisp.Object frame)
		{
			Console.WriteLine ("yo");
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("set-face-bold-p", MinArgs = 2)]
		public static Shelisp.Object Fset_face_bold_p (L l, Shelisp.Object face, Shelisp.Object bold_p, Shelisp.Object frame)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("set-face-italic-p", MinArgs = 2)]
		public static Shelisp.Object Fset_face_italic_p (L l, Shelisp.Object face, Shelisp.Object italic_p, Shelisp.Object frame)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("set-face-underline-p", MinArgs = 2)]
		public static Shelisp.Object Fset_face_underline_p (L l, Shelisp.Object face, Shelisp.Object underline_p, Shelisp.Object frame)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("set-face-inverse-video-p", MinArgs = 2)]
		public static Shelisp.Object Fset_face_inverse_video_p (L l, Shelisp.Object face, Shelisp.Object inverse_video_p, Shelisp.Object frame)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("invert-face", MinArgs = 1)]
		public static Shelisp.Object Finvert_face (L l, Shelisp.Object face, Shelisp.Object frame)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("face-foreground", MinArgs = 1)]
		public static Shelisp.Object Fface_foreground (L l, Shelisp.Object face, Shelisp.Object frame, Shelisp.Object inherit)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("face-background", MinArgs = 1)]
		public static Shelisp.Object Fface_background (L l, Shelisp.Object face, Shelisp.Object frame, Shelisp.Object inherit)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("face-stipple", MinArgs = 1)]
		public static Shelisp.Object Fface_stipple (L l, Shelisp.Object face, Shelisp.Object frame, Shelisp.Object inherit)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("face-bold-p", MinArgs = 1)]
		public static Shelisp.Object Fface_bold_p (L l, Shelisp.Object face, Shelisp.Object frame, Shelisp.Object inherit)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("face-italic-p", MinArgs = 1)]
		public static Shelisp.Object Fface_italic_p (L l, Shelisp.Object face, Shelisp.Object frame, Shelisp.Object inherit)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("face-underline-p", MinArgs = 1)]
		public static Shelisp.Object Fface_underline_p (L l, Shelisp.Object face, Shelisp.Object frame, Shelisp.Object inherit)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}

		[LispBuiltin ("face-inverse-video-p", MinArgs = 1)]
		public static Shelisp.Object Fface_invert_video_p (L l, Shelisp.Object face, Shelisp.Object frame, Shelisp.Object inherit)
		{
			Console.WriteLine ("not implemented"); return L.Qnil;
		}
	}
}
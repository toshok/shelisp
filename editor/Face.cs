using System;
using Shelisp;

namespace Shemacs.Editor {
	class Face : Shelisp.Object {

		// elisp manual says this is a macro?
		[LispBuiltin ("defface", MinArgs = 3)]
		public static Shelisp.Object Fdefface (L l, Shelisp.Object face, Shelisp.Object spec, Shelisp.Object doc, params Shelisp.Object[] rest)
		{
			Console.WriteLine ("defface not implemented"); return L.Qnil;
		}

		[LispBuiltin ("set-face-attribute", MinArgs = 2)]
		public static Shelisp.Object Fset_face_attribute (L l, Shelisp.Object face, Shelisp.Object frame, params Shelisp.Object[] arguments)
		{
			Console.WriteLine ("set-face-attribute not implemented"); return L.Qnil;
		}

		[LispBuiltin ("face-attribute-relative-p", MinArgs = 2)]
		public static Shelisp.Object Fface_attribute_relative_p (L l, Shelisp.Object attribute, Shelisp.Object value)
		{
			Console.WriteLine ("face-attribute-relative-p not implemented"); return L.Qnil;
		}

		[LispBuiltin ("face-all-attributes", MinArgs = 1)]
		public static Shelisp.Object Fface_all_attributes (L l, Shelisp.Object face, Shelisp.Object frame)
		{
			Console.WriteLine ("face-all-attributes not implemented"); return L.Qnil;
		}

		[LispBuiltin ("merge-face-attribute", MinArgs = 3)]
		public static Shelisp.Object Fmerge_face_attribute (L l, Shelisp.Object attribute, Shelisp.Object value1, Shelisp.Object value2)
		{
			Console.WriteLine ("merge-face-attribute not implemented"); return L.Qnil;
		}

		[LispBuiltin ("set-face-foreground", MinArgs = 2)]
		public static Shelisp.Object Fset_face_foreground (L l, Shelisp.Object face, Shelisp.Object color, Shelisp.Object frame)
		{
			Console.WriteLine ("set-face-foreground not implemented"); return L.Qnil;
		}

		[LispBuiltin ("set-face-background", MinArgs = 2)]
		public static Shelisp.Object Fset_face_background (L l, Shelisp.Object face, Shelisp.Object color, Shelisp.Object frame)
		{
			Console.WriteLine ("set-face-background not implemented"); return L.Qnil;
		}

		[LispBuiltin ("set-face-stipple", MinArgs = 2)]
		public static Shelisp.Object Fset_face_stipple (L l, Shelisp.Object face, Shelisp.Object pattern, Shelisp.Object frame)
		{
			Console.WriteLine ("set-face-stipple not implemented"); return L.Qnil;
		}

		[LispBuiltin ("set-face-font", MinArgs = 2)]
		public static Shelisp.Object Fset_face_font (L l, Shelisp.Object face, Shelisp.Object font, Shelisp.Object frame)
		{
			Console.WriteLine ("set-face-font not implemented"); return L.Qnil;
		}

		[LispBuiltin ("set-face-bold-p", MinArgs = 2)]
		public static Shelisp.Object Fset_face_bold_p (L l, Shelisp.Object face, Shelisp.Object bold_p, Shelisp.Object frame)
		{
			Console.WriteLine ("set-face-bold-p not implemented"); return L.Qnil;
		}

		[LispBuiltin ("set-face-italic-p", MinArgs = 2)]
		public static Shelisp.Object Fset_face_italic_p (L l, Shelisp.Object face, Shelisp.Object italic_p, Shelisp.Object frame)
		{
			Console.WriteLine ("set-face-italic-p not implemented"); return L.Qnil;
		}

		[LispBuiltin ("set-face-underline-p", MinArgs = 2)]
		public static Shelisp.Object Fset_face_underline_p (L l, Shelisp.Object face, Shelisp.Object underline_p, Shelisp.Object frame)
		{
			Console.WriteLine ("set-face-underline-p not implemented"); return L.Qnil;
		}

		[LispBuiltin ("set-face-inverse-video-p", MinArgs = 2)]
		public static Shelisp.Object Fset_face_inverse_video_p (L l, Shelisp.Object face, Shelisp.Object inverse_video_p, Shelisp.Object frame)
		{
			Console.WriteLine ("set-face-inverse-video-p not implemented"); return L.Qnil;
		}

		[LispBuiltin ("invert-face", MinArgs = 1)]
		public static Shelisp.Object Finvert_face (L l, Shelisp.Object face, Shelisp.Object frame)
		{
			Console.WriteLine ("invert-face not implemented"); return L.Qnil;
		}

		[LispBuiltin ("face-foreground", MinArgs = 1)]
		public static Shelisp.Object Fface_foreground (L l, Shelisp.Object face, Shelisp.Object frame, Shelisp.Object inherit)
		{
			Console.WriteLine ("face-foreground not implemented"); return L.Qnil;
		}

		[LispBuiltin ("face-background", MinArgs = 1)]
		public static Shelisp.Object Fface_background (L l, Shelisp.Object face, Shelisp.Object frame, Shelisp.Object inherit)
		{
			Console.WriteLine ("face-background not implemented"); return L.Qnil;
		}

		[LispBuiltin ("face-stipple", MinArgs = 1)]
		public static Shelisp.Object Fface_stipple (L l, Shelisp.Object face, Shelisp.Object frame, Shelisp.Object inherit)
		{
			Console.WriteLine ("face-stipple not implemented"); return L.Qnil;
		}

		[LispBuiltin ("face-bold-p", MinArgs = 1)]
		public static Shelisp.Object Fface_bold_p (L l, Shelisp.Object face, Shelisp.Object frame, Shelisp.Object inherit)
		{
			Console.WriteLine ("face-bold-p not implemented"); return L.Qnil;
		}

		[LispBuiltin ("face-italic-p", MinArgs = 1)]
		public static Shelisp.Object Fface_italic_p (L l, Shelisp.Object face, Shelisp.Object frame, Shelisp.Object inherit)
		{
			Console.WriteLine ("face-italic-p not implemented"); return L.Qnil;
		}

		[LispBuiltin ("face-underline-p", MinArgs = 1)]
		public static Shelisp.Object Fface_underline_p (L l, Shelisp.Object face, Shelisp.Object frame, Shelisp.Object inherit)
		{
			Console.WriteLine ("face-underline-p not implemented"); return L.Qnil;
		}

		[LispBuiltin ("face-inverse-video-p", MinArgs = 1)]
		public static Shelisp.Object Fface_invert_video_p (L l, Shelisp.Object face, Shelisp.Object frame, Shelisp.Object inherit)
		{
			Console.WriteLine ("face-inverse-video-p not implemented"); return L.Qnil;
		}



		// comes from xfaces.c
		[LispBuiltin (DocString = @"Set font selection order for face font selection to ORDER.
ORDER must be a list of length 4 containing the symbols `:width',
`:height', `:weight', and `:slant'.  Face attributes appearing
first in ORDER are matched first, e.g. if `:height' appears before
`:weight' in ORDER, font selection first tries to find a font with
a suitable height, and then tries to match the font weight.
Value is ORDER.")]
		public static Shelisp.Object Finternal_set_font_selection_order (L l, Shelisp.Object order)
		{
			Console.WriteLine ("internal-set-font-selection-order not implemented"); return L.Qnil;
		}

		// comes from xfaces.c
		[LispBuiltin (DocString = @"Define alternative font families to try in face font selection.
ALIST is an alist of (FAMILY ALTERNATIVE1 ALTERNATIVE2 ...) entries.
Each ALTERNATIVE is tried in order if no fonts of font family FAMILY can
be found.  Value is ALIST.")]
		public static Shelisp.Object Finternal_set_alternative_font_family_alist (L l, Shelisp.Object alist)
		{
			Console.WriteLine ("internal-set-alternative-font-family-alist not implemented"); return L.Qnil;
		}

		// comes from xfaces.c
		[LispBuiltin (DocString = @"Define alternative font registries to try in face font selection.
ALIST is an alist of (REGISTRY ALTERNATIVE1 ALTERNATIVE2 ...) entries.
Each ALTERNATIVE is tried in order if no fonts of font registry REGISTRY can
be found.  Value is ALIST.")]
		public static Shelisp.Object Finternal_set_alternative_font_registry_alist (L l, Shelisp.Object alist)
		{
			Console.WriteLine ("internal-set-alternative-font-registry-alist not implemented"); return L.Qnil;
		}

		// comes from xfaces.c
		[LispBuiltin (DocString = @"Return non-nil if FACE names a face.
FACE should be a symbol or string.
If optional second argument FRAME is non-nil, check for the
existence of a frame-local face with name FACE on that frame.
Otherwise check for the existence of a global face.")]
		public static Shelisp.Object Finternal_lisp_face_p (L l, Shelisp.Object face, Shelisp.Object frame)
		{
			Console.WriteLine ("internal-lisp-face-p not implemented"); return L.Qnil;
		}

		// comes from xfaces.c
		[LispBuiltin (DocString = "List of global face definitions (for internal use only.)")]
		public static Shelisp.Object Vface_new_frame_defaults = L.Qnil;
	}
}
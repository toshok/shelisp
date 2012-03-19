using Shelisp;

namespace Shelisp.Editor {

	public class Disp {

		[LispBuiltin]
		public static int baud_rate = 9600; // XXX


		[LispBuiltin]
		public static bool inverse_video = false; // XXX

		[LispBuiltin]
		public static bool visible_bell = false; // XXX

		[LispBuiltin]
		public static bool no_redraw_on_reenter = false; // XXX

		[LispBuiltin (DocString = @"*Maximum height for resizing mini-windows (the minibuffer and the echo area).
If a float, it specifies a fraction of the mini-window frame's height.
If an integer, it specifies a number of lines.")]
		public static float max_mini_window_height = 0.25f;


		[LispBuiltin (DocString = @"*Non-nil means mouse commands use dialog boxes to ask questions.
This applies to `y-or-n-p' and `yes-or-no-p' questions asked by commands
invoked by mouse clicks and mouse menu items.

On some platforms, file selection dialogs are also enabled if this is
non-nil.")]
		public static bool use_dialog_box = true;

		[LispBuiltin (DocString = @"*Non-nil means mouse commands use a file dialog to ask for files.
This applies to commands from menus and tool bar buttons even when
they are initiated from the keyboard.  If `use-dialog-box' is nil,
that disables the use of a file dialog, regardless of the value of
this variable.")]
		public static bool use_file_dialog = true;


		// this comes from xdisp.c
		[LispBuiltin (DocString = @"List of variables (symbols) which hold markers for overlay arrows.
The symbols on this list are examined during redisplay to determine
where to display overlay arrows.")]
		public static Shelisp.Object Voverlay_arrow_variable_list = new List (L.intern ("overlay-arrow-position"), L.Qnil);
	}

}
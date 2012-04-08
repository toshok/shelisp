using Shelisp;

namespace Shemacs.Editor {

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


		[LispBuiltin (DocString = @"Name of the window system that Emacs uses for the first frame.
The value is a symbol:
 nil for a termcap frame (a character-only terminal),
 'x' for an Emacs frame that is really an X window,
 'w32' for an Emacs frame that is a window on MS-Windows display,
 'ns' for an Emacs frame on a GNUstep or Macintosh Cocoa display,
 'pc' for a direct-write MS-DOS frame.

Use of this variable as a boolean is deprecated.  Instead,
use `display-graphic-p' or any of the other `display-*-p'
predicates which report frame's specific UI-related capabilities.")]
		public static Shelisp.Object Vinitial_window_system = L.Qnil;

		[LispBuiltin (DocString = @"The version number of the window system in use.
For X windows, this is 11.")]
		public static Shelisp.Object Vwindow_system_version = L.Qnil;

		// this comes from xdisp.c
		[LispBuiltin (DocString = @"List of variables (symbols) which hold markers for overlay arrows.
The symbols on this list are examined during redisplay to determine
where to display overlay arrows.")]
		public static Shelisp.Object Voverlay_arrow_variable_list = new List (L.intern ("overlay-arrow-position"), L.Qnil);

		// this comes from xdisp.c
		[LispBuiltin (DocString = @"Marker for where to display an arrow on top of the buffer text.
This must be the beginning of a line in order to work.
See also `overlay-arrow-string'.")]
		public static Shelisp.Object Voverlay_arrow_position = L.Qnil;

	}

}
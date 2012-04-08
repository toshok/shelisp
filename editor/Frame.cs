using Shelisp;

namespace Shemacs.Editor {
	class Frame : Object {

		public Frame (L l)
		{
		}

		public Shelisp.Object buffer_list;
		public Shelisp.Object buried_buffer_list;

		public static Shelisp.Object Vframe_list = L.Qnil;

		[LispBuiltin (DocString = "Return a list of all live frames.")]
		public static Shelisp.Object Fframe_list (L l)
		{
			var frames = Sequence.Fcopy_sequence (l, Vframe_list);
#if HAVE_WINDOW_SYSTEM
			if (FRAMEP (tip_frame))
				frames = Fdelq (tip_frame, frames);
#endif
			return frames;
		}

		[LispBuiltin (DocString = @"Non-nil if window system changes focus when you move the mouse.
You should set this variable to tell Emacs how your window manager
handles focus, since there is no way in general for Emacs to find out
automatically.  See also `mouse-autoselect-window'.")]
		public static bool focus_follows_mouse = false;

		[LispBuiltin (DocString = @"Alist of default values for frame creation.
These may be set in your init file, like this:
  (setq default-frame-alist '((width . 80) (height . 55) (menu-bar-lines . 1)))
These override values given in window system configuration data,
 including X Windows' defaults database.
For values specific to the first Emacs frame, see `initial-frame-alist'.
For window-system specific values, see `window-system-default-frame-alist'.
For values specific to the separate minibuffer frame, see
 `minibuffer-frame-alist'.
The `menu-bar-lines' element of the list controls whether new frames
 have menu bars; `menu-bar-mode' works by altering this element.
 Setting this variable does not affect existing frames, only new ones.")]
		public static Shelisp.Object Vdefault_frame_alist = L.Qnil;


		[LispBuiltin (DocString = @"If non-nil, clickable text is highlighted when mouse is over it.
If the value is an integer, highlighting is only shown after moving the
mouse, while keyboard input turns off the highlight even when the mouse
is over the clickable text.  However, the mouse shape still indicates
when the mouse is over clickable text.")]
		public static Shelisp.Object Vmouse_highlight = L.Qt;

		[LispBuiltin (DocString = @"If non-nil, make pointer invisible while typing.
The pointer becomes visible again when the mouse is moved.")]
		public static Shelisp.Object Vmake_pointer_invisible = L.Qt;

		[LispBuiltin (DocString = @"Functions to be run before deleting a frame.
The functions are run with one arg, the frame to be deleted.
See `delete-frame'.

Note that functions in this list may be called just before the frame is
actually deleted, or some time later (or even both when an earlier function
in `delete-frame-functions' (indirectly) calls `delete-frame'
recursively).")]
		public static Shelisp.Object Vdelete_frame_functions = L.Qnil;

		[LispBuiltin (DocString = @"Non-nil if Menu-Bar mode is enabled.
See the command `menu-bar-mode' for a description of this minor mode.
Setting this variable directly does not take effect;
either customize it (see the info node `Easy Customization')
or call the function `menu-bar-mode'.")]
		public static bool menu_bar_mode = true;

		[LispBuiltin (DocString = @"Non-nil if Tool-Bar mode is enabled.
See the command `tool-bar-mode' for a description of this minor mode.
Setting this variable directly does not take effect;
either customize it (see the info node `Easy Customization')
or call the function `tool-bar-mode'.")]
		public static bool tool_bar_mode = 
#if HAVE_WINDOW_SYSTEM
			tool_bar_mode = true;
#else
			tool_bar_mode = false;
#endif

	}

}
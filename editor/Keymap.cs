using System;
using Shelisp;

namespace Shemacs.Editor {

	public class Keymap : Shelisp.Object {
		[LispBuiltin]
		public static Shelisp.Object Fdefine_key (L l, Shelisp.Object keymap, Shelisp.Object key, Shelisp.Object binding)
		{
			// XXX more here
			return binding;
		}

		[LispBuiltin (MinArgs = 0, DocString = @"Construct and return a new keymap, of the form (keymap CHARTABLE . ALIST).
CHARTABLE is a char-table that holds the bindings for all characters
without modifiers.  All entries in it are initially nil, meaning
""command undefined"".  ALIST is an assoc-list which holds bindings for
function keys, mouse events, and any other things that appear in the
input stream.  Initially, ALIST is nil.

The optional arg STRING supplies a menu name for the keymap
in case you use it as a menu with `x-popup-menu'.")]
		public static Shelisp.Object Fmake_keymap (L l, [LispOptional] Shelisp.Object str)
		{
			Shelisp.Object tail;
			if (!L.NILP (str)) {
				Console.WriteLine ("str = {0}", str == null ? "null" : "not null");
				tail = new List (str, L.Qnil);
			}
			else
				tail = L.Qnil;
			return new List (L.intern ("keymap"),
					 new List (CharTable.Fmake_char_table (l, L.intern ("keymap"), L.Qnil), tail));
		}

		[LispBuiltin]
		public static Shelisp.Object Fkeymapp (L l, Shelisp.Object keymap)
		{
			return keymap is Keymap ? L.Qt : L.Qnil;
		}

		static Shelisp.Object make_sparse_keymap (Shelisp.Object prompt)
		{
			return new List (L.intern ("keymap"), L.Qnil);
		}

		[LispBuiltin]
		public static Shelisp.Object Fmake_sparse_keymap (L l, Shelisp.Object prompt)
		{
			return make_sparse_keymap (prompt);
		}

		static Shelisp.Object current_global_map = L.Qnil;

		[LispBuiltin]
		public static Shelisp.Object Fcurrent_global_map (L l)
		{
			return current_global_map;
		}

		[LispBuiltin (DocString = @"Modify KEYMAP to set its parent map to PARENT.
Return PARENT.  PARENT should be nil or another keymap.")]
		public static Shelisp.Object Fset_keymap_parent (L l, Shelisp.Object keymap, Shelisp.Object parent)
		{
			// XXX implement this..
			return parent;
		}

		[LispBuiltin (DocString = @"Call FUNCTION once for each event binding in KEYMAP.
FUNCTION is called with two arguments: the event that is bound, and
the definition it is bound to.  The event may be a character range.

If KEYMAP has a parent, the parent's bindings are included as well.
This works recursively: if the parent has itself a parent, then the
grandparent's bindings are also included and so on.
usage: (map-keymap FUNCTION KEYMAP)")]
		public static Shelisp.Object Fmap_keymap (L l, Shelisp.Object function, Shelisp.Object keymap, [LispOptional] Shelisp.Object sort_first)
		{
#if notyet
			if (! NILP (sort_first))
				return call2 (intern ("map-keymap-sorted"), function, keymap);

			map_keymap (keymap, map_keymap_call, function, NULL, 1);
#endif
			return keymap; // XXX
		}

		[LispBuiltin (DocString = "Default keymap to use when reading from the minibuffer.")]
		public static Shelisp.Object Vminibuffer_local_map = Keymap.make_sparse_keymap (L.Qnil);

		[LispBuiltin (DocString = @"The parent keymap of all `local-function-key-map' instances.
Function key definitions that apply to all terminal devices should go
here.  If a mapping is defined in both the current
`local-function-key-map' binding and this variable, then the local
definition will take precedence.")]
		public static Shelisp.Object Vfunction_key_map = Keymap.make_sparse_keymap (L.Qnil);

		[LispBuiltin (DocString = "Keymap defining bindings for special events to execute at low level.")]
		public static Shelisp.Object Vspecial_event_map = new List (L.intern ("keymap"), L.Qnil);

		[LispBuiltin (DocString = @"Keymap of key translations that can override keymaps.
This keymap works like `function-key-map', but comes after that,
and its non-prefix bindings override ordinary bindings.
Another difference is that it is global rather than keyboard-local.")]
		public static Shelisp.Object Vkey_translation_map = Keymap.make_sparse_keymap (L.Qnil);

		[LispBuiltin (DocString = @"Alist of keymaps to use for minor modes.
Each element looks like (VARIABLE . KEYMAP); KEYMAP is used to read
key sequences and look up bindings if VARIABLE's value is non-nil.
If two active keymaps bind the same key, the keymap appearing earlier
in the list takes precedence.")]
		public static Shelisp.Object Vminor_mode_map_alist = L.Qnil;

		[LispBuiltin (DocString = "Local keymap for the minibuffer when spaces are not allowed.")]
		public static Shelisp.Object Vminibuffer_local_ns_map = Keymap.make_sparse_keymap (L.Qnil);
		// XXX emacs has Fset_keymap_parent (Vminibuffer_local_ns_map, Vminibuffer_local_map);

		[LispBuiltin (DocString = @"Meta-prefix character code.
Meta-foo as command input turns into this character followed by foo.")]
		public static int meta_prefix_char = 033;
	}
}
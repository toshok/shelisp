using System;
using System.IO;
using Shelisp;

namespace Shemacs.Editor {

	public class Minibuffer {

		[LispBuiltin (DocString = @"Return common substring of all completions of STRING in COLLECTION.
Test each possible completion specified by COLLECTION
to see if it begins with STRING.  The possible completions may be
strings or symbols.  Symbols are converted to strings before testing,
see `symbol-name'.
All that match STRING are compared together; the longest initial sequence
common to all these matches is the return value.
If there is no match at all, the return value is nil.
For a unique match which is exact, the return value is t.

If COLLECTION is an alist, the keys (cars of elements) are the
possible completions.  If an element is not a cons cell, then the
element itself is the possible completion.
If COLLECTION is a hash-table, all the keys that are strings or symbols
are the possible completions.
If COLLECTION is an obarray, the names of all symbols in the obarray
are the possible completions.

COLLECTION can also be a function to do the completion itself.
It receives three arguments: the values STRING, PREDICATE and nil.
Whatever it returns becomes the value of `try-completion'.

If optional third argument PREDICATE is non-nil,
it is used to test each possible match.
The match is a candidate only if PREDICATE returns non-nil.
The argument given to PREDICATE is the alist element
or the symbol from the obarray.  If COLLECTION is a hash-table,
predicate is called with two arguments: the key and the value.
Additionally to this predicate, `completion-regexp-list'
is used to further constrain the set of candidates.")]
		public static Shelisp.Object Ftry_completion (L l, Shelisp.Object str, Shelisp.Object collection, [LispOptional] Shelisp.Object predicate)
		{
			Console.WriteLine ("try-completion (str = {0}, collection = {1}) not implemented", str, collection); return str;
		}


		[LispBuiltin]
		public static Shelisp.Object Fcompleting_read (L l, Shelisp.Object prompt, Shelisp.Object collection, [LispOptional] Shelisp.Object predicate, Shelisp.Object require_match,
							       Shelisp.Object initial_input, Shelisp.Object hist, Shelisp.Object def, Shelisp.Object inherit_input_method)
		{
			Console.Write (prompt.ToString("princ"));
			return (Shelisp.String)Console.ReadLine ();
		}

		[LispBuiltin]
		public static Shelisp.Object Fall_completions (L l, Shelisp.Object str, Shelisp.Object collection, Shelisp.Object predicate, Shelisp.Object hide_spaces)
		{
			Console.WriteLine ("all-completions (str = {0}) not implemented", str);
			return L.Qnil;
		}

		[LispBuiltin (DocString = @"Text properties that are added to minibuffer prompts.
These are in addition to the basic `field' property, and stickiness
properties.")]
		public static Shelisp.Object Vminibuffer_prompt_properties = new List (L.intern ("read-only"), new List (L.Qt, L.Qnil));

		[LispBuiltin (DocString = @"Read a string from the minibuffer, prompting with string PROMPT.
The optional second arg INITIAL-CONTENTS is an obsolete alternative to
  DEFAULT-VALUE.  It normally should be nil in new code, except when
  HIST is a cons.  It is discussed in more detail below.

Third arg KEYMAP is a keymap to use whilst reading;
  if omitted or nil, the default is `minibuffer-local-map'.

If fourth arg READ is non-nil, interpret the result as a Lisp object
  and return that object:
  in other words, do `(car (read-from-string INPUT-STRING))'

Fifth arg HIST, if non-nil, specifies a history list and optionally
  the initial position in the list.  It can be a symbol, which is the
  history list variable to use, or a cons cell (HISTVAR . HISTPOS).
  In that case, HISTVAR is the history list variable to use, and
  HISTPOS is the initial position for use by the minibuffer history
  commands.  For consistency, you should also specify that element of
  the history as the value of INITIAL-CONTENTS.  Positions are counted
  starting from 1 at the beginning of the list.

Sixth arg DEFAULT-VALUE, if non-nil, should be a string, which is used
  as the default to `read' if READ is non-nil and the user enters
  empty input.  But if READ is nil, this function does _not_ return
  DEFAULT-VALUE for empty input!  Instead, it returns the empty string.

  Whatever the value of READ, DEFAULT-VALUE is made available via the
  minibuffer history commands.  DEFAULT-VALUE can also be a list of
  strings, in which case all the strings are available in the history,
  and the first string is the default to `read' if READ is non-nil.

Seventh arg INHERIT-INPUT-METHOD, if non-nil, means the minibuffer inherits
 the current input method and the setting of `enable-multibyte-characters'.

If the variable `minibuffer-allow-text-properties' is non-nil,
 then the string which is returned includes whatever text properties
 were present in the minibuffer.  Otherwise the value has no text properties.

The remainder of this documentation string describes the
INITIAL-CONTENTS argument in more detail.  It is only relevant when
studying existing code, or when HIST is a cons.  If non-nil,
INITIAL-CONTENTS is a string to be inserted into the minibuffer before
reading input.  Normally, point is put at the end of that string.
However, if INITIAL-CONTENTS is \(STRING . POSITION), the initial
input is STRING, but point is placed at _one-indexed_ position
POSITION in the minibuffer.  Any integer value less than or equal to
one puts point at the beginning of the string.  *Note* that this
behavior differs from the way such arguments are used in `completing-read'
and some related functions, which use zero-indexing for POSITION.")]
		public static Shelisp.Object Fread_from_minibuffer (L l, Shelisp.Object prompt,
								    [LispOptional] Shelisp.Object initial_contents, Shelisp.Object keymap, Shelisp.Object read, Shelisp.Object hist, Shelisp.Object default_value, Shelisp.Object inherit_input_method)
		{
			Console.Write (prompt.ToString("princ"));
			var input = Console.ReadLine ();

			if (L.NILP (read))
				return (Shelisp.String)input;
			else {
				return Reader.ReadFromString (input);
			}
		}
	}

}

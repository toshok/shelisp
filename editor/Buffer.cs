using System;
using Shelisp;

namespace Shemacs.Editor {

	class RegionCache { }
	class Window { }
	class Overlay { }

	class BufferText {
		public const int BEG = 1;

		public BufferText ()
		{
			gap_size = 20;

#if notyet
			BLOCK_INPUT;
			/* We allocate extra 1-byte at the tail and keep it always '\0' for
			   anchoring a search.  */
			alloc_buffer_text (b, BUF_GAP_SIZE (b) + 1);
			UNBLOCK_INPUT;
			if (! BUF_BEG_ADDR (b))
				buffer_memory_full (BUF_GAP_SIZE (b) + 1);
#endif

			gpt = BEG;
			gpt_byte = BEG;

#if notyet
			*(BUF_GPT_ADDR (b)) = *(BUF_Z_ADDR (b)) = 0; /* Put an anchor '\0'.  */
#endif
			inhibit_shrinking = 0;

			z = BEG;
			z_byte = BEG;
			modiff = 1;
			chars_modiff = 1;
			overlay_modiff = 1;
			save_modiff = 1;
#if notyet
			intervals = 0;
#endif
			unchanged_modified = 1;
			overlay_unchanged_modified = 1;
			end_unchanged = 0;
			beg_unchanged = 0;

			markers = null;
		}

#if notyet
		/* Actual address of buffer contents.  If REL_ALLOC is defined,
		   this address might change when blocks are relocated which can
		   e.g. happen when malloc is called.  So, don't pass a pointer
		   into a buffer's text to functions that malloc.  */
		public byte *beg;
#endif

		public int gpt;		/* Char pos of gap in buffer.  */
		public int z;		/* Char pos of end of buffer.  */
		public int gpt_byte;		/* Byte pos of gap in buffer.  */
		public int z_byte;		/* Byte pos of end of buffer.  */
		public int gap_size;		/* Size of buffer's gap.  */
		public int modiff;			/* This counts buffer-modification events
							   for this buffer.  It is incremented for
							   each such event, and never otherwise
							   changed.  */
		public int chars_modiff;           /* This is modified with character change
						      events for this buffer.  It is set to
						      modiff for each such event, and never
						      otherwise changed.  */
		public int save_modiff;		/* Previous value of modiff, as of last
						   time buffer visited or saved a file.  */

		public int overlay_modiff;		/* Counts modifications to overlays.  */

		/* Minimum value of GPT - BEG since last redisplay that finished.  */
		public int beg_unchanged;

		/* Minimum value of Z - GPT since last redisplay that finished.  */
		public int end_unchanged;

		/* MODIFF as of last redisplay that finished; if it matches MODIFF,
		   beg_unchanged and end_unchanged contain no useful information.  */
		public int unchanged_modified;

		/* BUF_OVERLAY_MODIFF of current buffer, as of last redisplay that
		   finished; if it matches BUF_OVERLAY_MODIFF, beg_unchanged and
		   end_unchanged contain no useful information.  */
		public int overlay_unchanged_modified;

#if notyet
		/* Properties of this buffer's text.  */
		public INTERVAL intervals;
#endif

		/* The markers that refer to this buffer.
		   This is actually a single marker ---
		   successive elements in its marker `chain'
		   are the other markers referring to this buffer.
		   This is a singly linked unordered list, which means that it's
		   very cheap to add a marker to the list and it's also very cheap
		   to move a marker within a buffer.  */
		public Marker markers;

		/* Usually 0.  Temporarily set to 1 in decode_coding_gap to
		   prevent Fgarbage_collect from shrinking the gap and losing
		   not-yet-decoded bytes.  */
		public int inhibit_shrinking;
	}

	class Buffer : Shelisp.Object {
		// XXX this should be a command
		[LispBuiltin]
		public static Shelisp.Object Fmake_variable_buffer_local (L l, Shelisp.Object variable)
		{
			// XXX no-op for now
			return variable;
		}

		[LispBuiltin (DocString = @"Return non-nil if OBJECT is a buffer which has not been killed.
Value is nil if OBJECT is not a buffer or if it has been killed.")]
		public static Shelisp.Object Fbuffer_live_p (L l, Shelisp.Object o)
		{
			return ((o is Buffer) && !L.NILP (((Buffer)o).name)) ? L.Qt : L.Qnil;
		}

		/* Like Fassoc, but use Fstring_equal to compare
		   (which ignores text properties),
		   and don't ever QUIT.  */
		static Shelisp.Object assoc_ignore_text_properties (L l, Shelisp.Object key, Shelisp.Object list) {
			Shelisp.Object tail;
			for (tail = list; L.CONSP (tail); tail = L.CDR (tail)) {
				Shelisp.Object elt, tem;
				elt = L.CAR (tail);
				tem = Shelisp.String.Fstring_equal (l, List.Fcar (l, elt), key);
				if (!L.NILP (tem))
					return elt;
			}
			return L.Qnil;
		}

		[LispBuiltin (DocString = @"Return the buffer named BUFFER-OR-NAME.
BUFFER-OR-NAME must be either a string or a buffer.  If BUFFER-OR-NAME
is a string and there is no buffer with that name, return nil.  If
BUFFER-OR-NAME is a buffer, return it as given.")]
		public static Shelisp.Object Fget_buffer (L l, Shelisp.Object buffer_or_name)
		{
			if (buffer_or_name is Buffer)
				return buffer_or_name;
			//CHECK_STRING (buffer_or_name);
			return List.Fcdr (l, assoc_ignore_text_properties (l, buffer_or_name, Buffer.Vbuffer_alist));
		}

		[LispBuiltin (DocString = @"Return the buffer specified by BUFFER-OR-NAME, creating a new one if needed.
If BUFFER-OR-NAME is a string and a live buffer with that name exists,
return that buffer.  If no such buffer exists, create a new buffer with
that name and return it.  If BUFFER-OR-NAME starts with a space, the new
buffer does not keep undo information.
If BUFFER-OR-NAME is a buffer instead of a string, return it as given,
even if it is dead.  The return value is never nil.")]
		public static Shelisp.Object Fget_buffer_create (L l, Shelisp.Object buffer_or_name)
		{
			Shelisp.Object buffer;

			buffer = Fget_buffer (l, buffer_or_name);
			if (!L.NILP (buffer))
				return buffer;

			if (buffer_or_name is Shelisp.String) {
				string name = (Shelisp.String)buffer_or_name;

				if (name.Length == 0)
					throw new Exception ("Empty string for buffer name is not allowed");

				buffer = new Buffer(name);
			}
			else
				throw new Exception ();

			Buffer buf = (Buffer)buffer;

			/* Put this on the chain of all buffers including killed ones.  */
			buf.next = all_buffers;
			all_buffers = buf;

			/* Put this in the alist of all live buffers.  */
			Buffer.Vbuffer_alist = List.Fnconc (l,
							    Buffer.Vbuffer_alist,
							    new List (new List (buf.name, buffer), L.Qnil));

#if notyet
			/* And run buffer-list-update-hook.  */
			if (!L.NILP (L.Vrun_hooks))
				L.call (L.Vrun_hooks, L.Qbuffer_list_update_hook);

			/* An error in calling the function here (should someone redefine it)
			   can lead to infinite regress until you run out of stack.  rms
			   says that's not worth protecting against.  */
			if (!L.NILP (L.Ffboundp (L.Qucs_set_table_for_input)))
				/* buffer is on buffer-alist, so no gcpro.  */
				L.call (L.Qucs_set_table_for_input, buf);
#endif

			return buffer;
		}

		[LispBuiltin]
		public static Shelisp.Object Fset_buffer (L l, Shelisp.Object buffer_or_name)
		{
			Shelisp.Object buf = Fget_buffer (l, buffer_or_name);
			if (L.NILP (buf))
				throw new Exception ("unknown buffer");

			Buffer.SetBuffer ((Buffer)buf);
			return buf;
		}

#if notyet
		public static Func<Lisp.Object,Lisp.Object> Fbuffer_list;
		public static Lisp.Subr Sbuffer_list = L.DEFUN<Lisp.Object,Lisp.Object> (lisp_name: "buffer-list",
											 doc:  "Return a list of all existing live buffers. " +
											 "If the optional arg FRAME is a frame, we return the buffer list in the " +
											 "proper order for that frame: the buffers show in FRAME come first," +
											 "followed by the rest of the buffers.",
											 min_args: 1,
											 func: Fbuffer_list = (Lisp.Object frame) => {
												 Lisp.Object general = List.Fmapcar (L.Qcdr, L.Vbuffer_alist);

												 if (L.FRAMEP (frame)) {
													 Lisp.Object framelist, prevlist, tail;

													 //CHECK_FRAME (frame);
													 framelist = L.Fcopy_sequence (L.XFRAME (frame).buffer_list);
													 prevlist = L.Fnreverse (L.Fcopy_sequence,
																 (L.XFRAME (frame).buried_buffer_list));

													 /* Remove from GENERAL any buffer that duplicates one in
													    FRAMELIST or PREVLIST.  */
													 tail = framelist;
													 while (L.CONSP (tail)) {
														 general = L.Fdelq (L.XCAR (tail), general);
														 tail = L.XCDR (tail);
													 }
													 tail = prevlist;
													 while (L.CONSP (tail)) {
														 general = L.Fdelq (L.XCAR (tail), general);
														 tail = L.XCDR (tail);
													 }

													 return L.Fnconc (new Lisp.Object[] {
															 framelist,
															 general,
															 prevlist
														 });
												 }
												 else
													 return general;
											 });

		public static Func<Lisp.Object,Lisp.Object> Fget_file_buffer;
		public static Lisp.Subr Sget_file_buffer = L.DEFUN<Lisp.Object,Lisp.Object> (lisp_name: "get-file-buffer",
											     doc: "Return the buffer visiting file FILENAME (a string)." +
											     " The buffer's `buffer-file-name' must match exactly the expansion of FILENAME." +
											     " If there is no such live buffer, return nil." + 
											     " See also `find-buffer-visiting'.",
											     min_args: 1,
											     func: Fget_file_buffer = (Lisp.Object filename) => {
												     Lisp.Object tail, buf, tem;
												     Lisp.Object handler;

												     //CHECK_STRING (filename);
												     filename = L.Fexpand_file_name (filename, L.Qnil);

												     /* If the file name has special constructs in it,
													call the corresponding file handler.  */
												     handler = L.Ffind_file_name_handler (filename, L.Qget_file_buffer);
												     if (!L.NILP (handler)) {
													     Lisp.Object handled_buf = L.call (handler, L.Qget_file_buffer, filename);
													     return L.BUFFERP (handled_buf) ? handled_buf : L.Qnil;
												     }

												     for (tail = L.Vbuffer_alist; L.CONSP (tail); tail = L.XCDR (tail)) {
													     buf = List.Fcdr (L.XCAR (tail));
													     if (!L.BUFFERP (buf)) continue;
													     if (!L.STRINGP (L.XBUFFER(buf).filename)) continue;
													     tem = L.Fstring_equal (L.XBUFFER(buf).filename, filename);
													     if (!L.NILP (tem))
														     return buf;
												     }
												     return L.Qnil;
											     });

		public static Func<Lisp.Object,Lisp.Object> Fget_buffer_create;
		public static Lisp.Subr Sget_buffer_create = L.DEFUN<Lisp.Object,Lisp.Object> (lisp_name: "get-buffer-create",
											       doc: "Return the buffer specified by BUFFER-OR-NAME, creating a new one if needed." +
											       " If BUFFER-OR-NAME is a string and a live buffer with that name exists," +
											       " return that buffer.  If no such buffer exists, create a new buffer with" +
											       " that name and return it.  If BUFFER-OR-NAME starts with a space, the new" +
											       " buffer does not keep undo information." + 
											       " If BUFFER-OR-NAME is a buffer instead of a string, return it as given," +
											       " even if it is dead.  The return value is never nil.",
											       min_args: 1,
											       func: Fget_buffer_create = (Lisp.Object buffer_or_name) => {
												       Lisp.Object buffer, name;
												       Buffer b;

												       buffer = Fget_buffer (buffer_or_name);
												       if (!L.NILP (buffer))
													       return buffer;

#if notyet
												       if (L.SCHARS (buffer_or_name) == 0)
													       error ("Empty string for buffer name is not allowed");
#endif

												       name = L.Fcopy_sequence (buffer_or_name);
#if notyet
												       STRING_SET_INTERVALS (name, NULL_INTERVAL);
#endif

												       b = new Buffer(name);
#if notyet
												       /* Put this on the chain of all buffers including killed ones.  */
												       b->header.next.buffer = all_buffers;
												       all_buffers = b;
#endif

												       /* Put this in the alist of all live buffers.  */
												       L.XSETBUFFER (buffer, b);
												       L.Vbuffer_alist = L.Fnconc (new Lisp.Object[] {
														       L.Vbuffer_alist,
														       L.Fcons (L.Fcons (name, buffer), L.Qnil)
													       });

												       /* And run buffer-list-update-hook.  */
												       if (!L.NILP (L.Vrun_hooks))
													       L.call (L.Vrun_hooks, L.Qbuffer_list_update_hook);

												       /* An error in calling the function here (should someone redefine it)
													  can lead to infinite regress until you run out of stack.  rms
													  says that's not worth protecting against.  */
												       if (!L.NILP (L.Ffboundp (L.Qucs_set_table_for_input)))
													       /* buffer is on buffer-alist, so no gcpro.  */
													       L.call (L.Qucs_set_table_for_input, buffer);

												       return buffer;
											       });

		public static Func<Lisp.Object,Lisp.Object,Lisp.Object,Lisp.Object> Fmake_indirect_buffer;
		public static Lisp.Subr Smake_indirect_buffer = L.DEFUN<Lisp.Object,Lisp.Object,Lisp.Object,Lisp.Object> (lisp_name: "make-indirect-buffer",
															  min_args: 2,
															  doc: "Create and return an indirect buffer for buffer BASE-BUFFER, named NAME." +
															  " BASE-BUFFER should be a live buffer, or the name of an existing buffer." +
															  " NAME should be a string which is not the name of an existing buffer." +
															  " Optional argument CLONE non-nil means preserve BASE-BUFFER's state," + 
															  " such as major and minor modes, in the indirect buffer." + 
															  " CLONE nil means the indirect buffer's state is reset to default values.",
															  func: Fmake_indirect_buffer = (Lisp.Object base_buffer, Lisp.Object name, Lisp.Object clone) => {
																  Lisp.Object buf, tem;
																  Buffer b;

																  //CHECK_STRING (name);
																  buf = Fget_buffer (name);
#if notyet
																  if (!L.NILP (buf))
																	  error ("Buffer name `%s' is in use", L.SDATA (name));
#endif

																  tem = base_buffer;
																  base_buffer = Fget_buffer (base_buffer);
#if notyet
																  if (L.NILP (base_buffer))
																	  error ("No such buffer: `%s'", SDATA (tem));
																  if (L.NILP (L.XBUFFER (base_buffer).name))
																	  error ("Base buffer has been killed");

																  if (L.SCHARS (name) == 0)
																	  error ("Empty string for buffer name is not allowed");
#endif

																  name = L.Fcopy_sequence (name);
#if notyet
																  STRING_SET_INTERVALS (name, NULL_INTERVAL);
#endif

																  b = new Buffer (name, base_buffer);

#if notyet
																  /* Put this on the chain of all buffers including killed ones.  */
																  b->header.next.buffer = all_buffers;
																  all_buffers = b;
#endif

																  /* Put this in the alist of all live buffers.  */
																  L.XSETBUFFER (buf, b);
																  L.Vbuffer_alist = L.Fnconc (new Lisp.Object[] {
																		  L.Vbuffer_alist,
																		  L.Fcons (L.Fcons (name, buf), L.Qnil)
																	  });

																  if (L.NILP (clone)) {
																	  /* Give the indirect buffer markers for its narrowing.  */
																	  b.pt_marker = L.Fmake_marker ();
																	  Marker.SetBoth (b.pt_marker, buf, b.pt, b.pt_byte);
																	  b.begv_marker = L.Fmake_marker ();
																	  Marker.SetBoth (b.begv_marker, buf, b.begv, b.begv_byte);
																	  b.zv_marker = L.Fmake_marker ();
																	  Marker.SetBoth (b.zv_marker, buf, b.zv, b.zv_byte);
																	  L.XMARKER (b.zv_marker).insertion_type = 1;
																  }
																  else {
																	  Buffer old_b = Buffer.CurrentBuffer;

																	  b.ClonePerBufferValues (b.base_buffer);
																	  b.filename = L.Qnil;
																	  b.file_truename = L.Qnil;
																	  b.display_count = 0;
																	  b.backed_up = L.Qnil;
																	  b.auto_save_file_name = L.Qnil;
																	  SetBuffer_1 (b);
																	  L.Fset (L.intern ("buffer-save-without-query"), L.Qnil);
																	  L.Fset (L.intern ("buffer-file-number"), L.Qnil);
																	  L.Fset (L.intern ("buffer-stale-function"), L.Qnil);
																	  SetBuffer_1 (old_b);
																  }


																  /* Run buffer-list-update-hook.  */
																  if (!L.NILP (L.Vrun_hooks))
																	  L.call (L.Vrun_hooks, L.Qbuffer_list_update_hook);

																  return buf;
															  });
#endif

		public Buffer (Shelisp.Object name, Shelisp.Object buffer)
		{
			this.base_buffer = (((Buffer)buffer).base_buffer != null
					    ? ((Buffer)buffer).base_buffer
					    : ((Buffer)buffer));

			/* Use the base buffer's text object.  */
			text = base_buffer.text;

			pt = base_buffer.pt;
			begv = base_buffer.begv;
			zv = base_buffer.zv;
			pt_byte = base_buffer.pt_byte;
			begv_byte = base_buffer.begv_byte;
			zv_byte = base_buffer.zv_byte;

#if notyet
			newline_cache = 0;
			width_run_cache = 0;
#endif
			width_table = L.Qnil;

			Reset ();
			ResetLocalVariables (permanent_too: true);

			mark = new Marker ();
			this.name = name;

			/* The multibyte status belongs to the base buffer.  */
			enable_multibyte_characters = base_buffer.enable_multibyte_characters;

			/* Make sure the base buffer has markers for its narrowing.  */
			if (L.NILP (base_buffer.pt_marker)) {
#if notyet
				eassert (NILP (BVAR (base_buffer, begv_marker)));
				eassert (NILP (BVAR (base_buffer, zv_marker)));
#endif

				base_buffer.pt_marker = new Marker ();
				Marker.SetBoth (base_buffer.pt_marker, buffer,
						base_buffer.pt,
						base_buffer.pt_byte);

				base_buffer.begv_marker = new Marker ();
				Marker.SetBoth (base_buffer.begv_marker, buffer,
						base_buffer.begv,
						base_buffer.begv_byte);

				base_buffer.zv_marker = new Marker ();
				Marker.SetBoth (base_buffer.zv_marker, buffer,
						base_buffer.zv,
						base_buffer.zv_byte);
				((Marker)base_buffer.zv_marker).Type = Marker.InsertionType.MarkerAdvancesOnTextInsertion;
			}
		}

		public Buffer (Shelisp.Object name)
		{
			/* An ordinary buffer uses its own struct buffer_text.  */
			text = new BufferText();

			pt = BufferText.BEG;
			begv = BufferText.BEG;
			zv = BufferText.BEG;
			pt_byte = BufferText.BEG;
			begv_byte = BufferText.BEG;
			zv_byte = BufferText.BEG;

#if notyet
			newline_cache = 0;
			width_run_cache = 0;
#endif
			width_table = L.Qnil;
			prevent_redisplay_optimizations_p = true;

			/* An ordinary buffer normally doesn't need markers
			   to handle BEGV and ZV.  */
			pt_marker = L.Qnil;
			begv_marker = L.Qnil;
			zv_marker = L.Qnil;

			undo_list = (L.SREF (name, 0) != ' ') ? L.Qnil : L.Qt;

			Reset ();
			ResetLocalVariables (permanent_too: true);

			mark = new Marker ();

			this.name = name;
		}

		public static Buffer CurrentBuffer { get; set; }

		public static Buffer BufferDefaults { get; set; }

		public static Buffer BufferLocalFlags { get; set; }

		public static Buffer BufferLocalSymbols { get; set; }

		/* Clone per-buffer values of buffer FROM.

		   Buffer TO gets the same per-buffer values as FROM, with the
		   following exceptions: (1) TO's name is left untouched, (2) markers
		   are copied and made to refer to TO, and (3) overlay lists are
		   copied.  */

		public void ClonePerBufferValues (Buffer from)
		{
		}

		public void DelegateAllOverlays ()
		{
		}

		public void Reset ()
		{
		}

		public void ResetLocalVariables (bool permanent_too)
		{
		}

		public void EvaporateOverlays (int position)
		{
		}

#if notyet
		extern ptrdiff_t overlays_at (int pos, int extend, Shelisp.Object **vec_ptr,
					      ptrdiff_t *len_ptr, int *next_ptr,
					      int *prev_ptr, int change_req);
		extern ptrdiff_t sort_overlays (Shelisp.Object *, ptrdiff_t, struct window *);
#endif

		public void RecenterOverlayLists (int position)
		{
			throw new NotImplementedException ();
		}

		public int OverlayStrings (int position, Window w, out byte[] str)
		{
			throw new NotImplementedException ();
		}

		public void ValidateRegion (Shelisp.Object b, Shelisp.Object e)
		{
			throw new NotImplementedException ();
		}

		public static void SetBuffer (Buffer b)
		{
			if (CurrentBuffer != b)
				SetBuffer_1 (b);
		}

		public static void SetBuffer_1 (Buffer b)
		{
			Buffer.CurrentBuffer = b;
		}

		public static void RecordBuffer (Shelisp.Object buffer)
		{
			throw new NotImplementedException ();
		}

		public static void BufferSlotTypMismatch (Shelisp.Object newval, int type)
		{
		}

		public void FixOverlaysBefore (Buffer buffer, int prev, int pos)
		{
		}

#if not_yet
		/* Get overlays at POSN into array OVERLAYS with NOVERLAYS elements.
		   If NEXTP is non-NULL, return next overlay there.
		   See overlay_at arg CHANGE_REQ for meaning of CHRQ arg.  */

		define GET_OVERLAYS_AT(posn, overlays, noverlays, nextp, chrq)		\
			do {									\
				ptrdiff_t maxlen = 40;							\
				overlays = (Shelisp.Object *) alloca (maxlen * sizeof (Shelisp.Object));	\
				noverlays = overlays_at (posn, 0, &overlays, &maxlen,		\
							 nextp, NULL, chrq);				\
				if (noverlays > maxlen)						\
					{									\
						maxlen = noverlays;						\
						overlays = (Shelisp.Object *) alloca (maxlen * sizeof (Shelisp.Object)); \
						noverlays = overlays_at (posn, 0, &overlays, &maxlen,		\
									 nextp, NULL, chrq);			\
					}									\
			} while (0)

				EXFUN (Fbuffer_live_p, 1);
		EXFUN (Fbuffer_name, 1);
		EXFUN (Fnext_overlay_change, 1);
		EXFUN (Fbuffer_local_value, 2);

		extern Shelisp.Object Qbefore_change_functions;
		extern Shelisp.Object Qafter_change_functions;
		extern Shelisp.Object Qfirst_change_hook;
#endif

		/* This points to the text that used for this buffer.
		   In an indirect buffer, this is a reference to the 'text' field of another buffer.  */
		BufferText text;

		/* Char position of point in buffer.  */
		int pt;
		/* Byte position of point in buffer.  */
		int pt_byte;
		/* Char position of beginning of accessible range.  */
		int begv;
		/* Byte position of beginning of accessible range.  */
		int begv_byte;
		/* Char position of end of accessible range.  */
		int zv;
		/* Byte position of end of accessible range.  */
		int zv_byte;

		/* In an indirect buffer, this points to the base buffer.
		   In an ordinary buffer, it is null.  */
		Buffer base_buffer;

		const int MAX_PER_BUFFER_VARS = 50;
		byte[] local_flags = new byte[MAX_PER_BUFFER_VARS];

		/* Set to the modtime of the visited file when read or written.
		   -1 means visited file was nonexistent.
		   0 means visited file modtime unknown; in no case complain
		   about any mismatch on next save attempt.  */
		DateTime modtime;

		/* Size of the file when modtime was set.  This is used to detect the
		   case where the file grew while we were reading it, so the modtime
		   is still the same (since it's rounded up to seconds) but we're actually
		   not up-to-date.  -1 means the size is unknown.  Only meaningful if
		   modtime is actually set.  */
		long modtime_size;
		/* The value of text->modiff at the last auto-save.  */
		int auto_save_modified;
		/* The value of text->modiff at the last display error.
		   Redisplay of this buffer is inhibited until it changes again.  */
		int display_error_modiff;
		/* The time at which we detected a failure to auto-save,
		   Or 0 if we didn't have a failure.  */
		DateTime auto_save_failure_time;
		/* Position in buffer at which display started
		   the last time this buffer was displayed.  */
		int last_window_start;

		/* Set nonzero whenever the narrowing is changed in this buffer.  */
		bool clip_changed;

		/* If the long line scan cache is enabled (i.e. the buffer-local
		   variable cache-long-line-scans is non-nil), newline_cache
		   points to the newline cache, and width_run_cache points to the
		   width run cache.

		   The newline cache records which stretches of the buffer are
		   known *not* to contain newlines, so that they can be skipped
		   quickly when we search for newlines.

		   The width run cache records which stretches of the buffer are
		   known to contain characters whose widths are all the same.  If
		   the width run cache maps a character to a value > 0, that value is
		   the character's width; if it maps a character to zero, we don't
		   know what its width is.  This allows compute_motion to process
		   such regions very quickly, using algebra instead of inspecting
		   each character.   See also width_table, below.  */
		RegionCache newline_cache;
		RegionCache width_run_cache;

		/* true means don't use redisplay optimizations for
		   displaying this buffer.  */
		bool prevent_redisplay_optimizations_p;

		/* List of overlays that end at or before the current center,
		   in order of end-position.  */
		Overlay overlays_before;

		/* List of overlays that end after  the current center,
		   in order of start-position.  */
		Overlay overlays_after;

		/* Position where the overlay lists are centered.  */
		int overlay_center;

		/* Changes in the buffer are recorded here for undo.
		   t means don't record anything.
		   This information belongs to the base buffer of an indirect buffer,
		   But we can't store it in the  struct buffer_text
		   because local variables have to be right in the  struct buffer.
		   So we copy it around in set_buffer_internal.
		   This comes before `name' because it is marked in a special way.  */
		Shelisp.Object undo_list;

		/* The name of this buffer.  */
		public Shelisp.Object name;

		/* The name of the file visited in this buffer, or nil.  */
		Shelisp.Object filename;
		/* Dir for expanding relative file names.  */
		Shelisp.Object directory;
		/* True if this buffer has been backed up (if you write to the
		   visited file and it hasn't been backed up, then a backup will
		   be made).  */
		/* This isn't really used by the C code, so could be deleted.  */
		Shelisp.Object backed_up;
		/* Length of file when last read or saved.
		   -1 means auto saving turned off because buffer shrank a lot.
		   -2 means don't turn off auto saving if buffer shrinks.
		   (That value is used with buffer-swap-text.)
		   This is not in the  struct buffer_text
		   because it's not used in indirect buffers at all.  */
		Shelisp.Object save_length;
		/* File name used for auto-saving this buffer.
		   This is not in the  struct buffer_text
		   because it's not used in indirect buffers at all.  */
		Shelisp.Object auto_save_file_name;

		/* Non-nil if buffer read-only.  */
		Shelisp.Object read_only;
		/* "The mark".  This is a marker which may
		   point into this buffer or may point nowhere.  */
		Shelisp.Object mark;

		/* Alist of elements (SYMBOL . VALUE-IN-THIS-BUFFER) for all
		   per-buffer variables of this buffer.  For locally unbound
		   symbols, just the symbol appears as the element.  */
		Shelisp.Object local_var_alist;

		/* Symbol naming major mode (eg, lisp-mode).  */
		[LispBuiltin]
		public static Shelisp.Object Vmajor_mode = L.Qnil;

		public Shelisp.Object major_mode;

		/* Pretty name of major mode (eg, "Lisp"). */
		Shelisp.Object mode_name;

		/* Mode line element that controls format of mode line.  */
		[LispBuiltin]
		public static Shelisp.Object Vmode_line_format = L.Qnil;

		public Shelisp.Object mode_line_format;

		/* Analogous to mode_line_format for the line displayed at the top
		   of windows.  Nil means don't display that line.  */
		Shelisp.Object header_line_format;

		/* Keys that are bound local to this buffer.  */
		Shelisp.Object keymap;
		/* This buffer's local abbrev table.  */
		Shelisp.Object abbrev_table;
		/* This buffer's syntax table.  */
		Shelisp.Object syntax_table;
		/* This buffer's category table.  */
		Shelisp.Object category_table;

		/* Values of several buffer-local variables.  */
		/* tab-width is buffer-local so that redisplay can find it
		   in buffers that are not current.  */
		private Shelisp.Object case_fold_search = L.Qnil;
		[LispBuiltin]
		public static Shelisp.Object Vcase_fold_search {
			get { return Buffer.CurrentBuffer.case_fold_search; }
			set { Buffer.CurrentBuffer.case_fold_search = value; }
		}

		private Shelisp.Object tab_width = L.Qnil;
		[LispBuiltin]
		public static Shelisp.Object Vtab_width {
			get { return Buffer.CurrentBuffer.tab_width; }
			set { Buffer.CurrentBuffer.tab_width = value; }
		}

		private Shelisp.Object fill_column = L.Qnil;
		[LispBuiltin]
		public static Shelisp.Object Vfill_column {
			get { return Buffer.CurrentBuffer.fill_column; }
			set { Buffer.CurrentBuffer.fill_column = value; }
		}


		private Shelisp.Object left_margin = L.Qnil;
		[LispBuiltin]
		public static Shelisp.Object Vleft_margin {
			get { return Buffer.CurrentBuffer.left_margin; }
			set { Buffer.CurrentBuffer.left_margin = value; }
		}


		/* Function to call when insert space past fill column.  */
		Shelisp.Object auto_fill_function;

		/* Case table for case-conversion in this buffer.
		   This char-table maps each char into its lower-case version.  */
		Shelisp.Object downcase_table;
		/* Char-table mapping each char to its upper-case version.  */
		Shelisp.Object upcase_table;
		/* Char-table for conversion for case-folding search.  */
		Shelisp.Object case_canon_table;
		/* Char-table of equivalences for case-folding search.  */
		Shelisp.Object case_eqv_table;

		/* Non-nil means do not display continuation lines.  */
		private Shelisp.Object truncate_lines = L.Qnil;
		[LispBuiltin]
		public static Shelisp.Object Vtruncate_lines {
			get { return Buffer.CurrentBuffer.truncate_lines; }
			set { Buffer.CurrentBuffer.truncate_lines = value; }
		}

		/* Non-nil means to use word wrapping when displaying continuation lines.  */
		private Shelisp.Object word_wrap = L.Qnil;
		[LispBuiltin]
		public static Shelisp.Object Vword_wrap {
			get { return Buffer.CurrentBuffer.word_wrap; }
			set { Buffer.CurrentBuffer.word_wrap = value; }
		}

		/* Non-nil means display ctl chars with uparrow.  */
		private Shelisp.Object ctl_arrow = L.Qnil;
		[LispBuiltin]
		public static Shelisp.Object Vctl_arrow {
			get { return Buffer.CurrentBuffer.ctl_arrow; }
			set { Buffer.CurrentBuffer.ctl_arrow = value; }
		}
		/* Non-nil means reorder bidirectional text for display in the
		   visual order.  */
		private Shelisp.Object bidi_display_reordering = L.Qnil;
		[LispBuiltin]
		public static Shelisp.Object Vbidi_display_reordering {
			get { return Buffer.CurrentBuffer.bidi_display_reordering; }
			set { Buffer.CurrentBuffer.bidi_display_reordering = value; }
		}

		/* If non-nil, specifies which direction of text to force in all the
		   paragraphs of the buffer.  Nil means determine paragraph
		   direction dynamically for each paragraph.  */
		private Shelisp.Object bidi_paragraph_direction = L.Qnil;
		[LispBuiltin]
		public static Shelisp.Object Vbidi_paragraph_direction {
			get { return Buffer.CurrentBuffer.bidi_paragraph_direction; }
			set { Buffer.CurrentBuffer.bidi_paragraph_direction = value; }
		}

		/* Non-nil means do selective display;
		   see doc string in syms_of_buffer (buffer.c) for details.  */
		Shelisp.Object selective_display;
		/* Non-nil means show ... at end of line followed by invisible lines.  */
		private Shelisp.Object selective_display_ellipses = L.Qnil;
		[LispBuiltin]
		public static Shelisp.Object Vselective_display_ellipses {
			get { return Buffer.CurrentBuffer.selective_display_ellipses; }
			set { Buffer.CurrentBuffer.selective_display_ellipses = value; }
		}
		/* Alist of (FUNCTION . STRING) for each minor mode enabled in buffer.  */
		Shelisp.Object minor_modes;
		/* t if "self-insertion" should overwrite; `binary' if it should also
		   overwrite newlines and tabs - for editing executables and the like.  */
		Shelisp.Object overwrite_mode;
		/* non-nil means abbrev mode is on.  Expand abbrevs automatically.  */
		Shelisp.Object abbrev_mode;
		/* Display table to use for text in this buffer.  */
		Shelisp.Object display_table;
		/* t means the mark and region are currently active.  */
		Shelisp.Object mark_active;

		/* Non-nil means the buffer contents are regarded as multi-byte
		   form of characters, not a binary code.  */
		Shelisp.Object enable_multibyte_characters;

		/* Coding system to be used for encoding the buffer contents on
		   saving.  */
		Shelisp.Object buffer_file_coding_system;

		/* List of symbols naming the file format used for visited file.  */
		Shelisp.Object file_format;

		/* List of symbols naming the file format used for auto-save file.  */
		Shelisp.Object auto_save_file_format;

		/* True if the newline position cache and width run cache are
		   enabled.  See search.c and indent.c.  */
		Shelisp.Object cache_long_line_scans;

		/* If the width run cache is enabled, this table contains the
		   character widths width_run_cache (see above) assumes.  When we
		   do a thorough redisplay, we compare this against the buffer's
		   current display table to see whether the display table has
		   affected the widths of any characters.  If it has, we
		   invalidate the width run cache, and re-initialize width_table.  */
		Shelisp.Object width_table;

		/* In an indirect buffer, or a buffer that is the base of an
		   indirect buffer, this holds a marker that records
		   PT for this buffer when the buffer is not current.  */
		Shelisp.Object pt_marker;

		/* In an indirect buffer, or a buffer that is the base of an
		   indirect buffer, this holds a marker that records
		   BEGV for this buffer when the buffer is not current.  */
		Shelisp.Object begv_marker;

		/* In an indirect buffer, or a buffer that is the base of an
		   indirect buffer, this holds a marker that records
		   ZV for this buffer when the buffer is not current.  */
		Shelisp.Object zv_marker;

		/* This holds the point value before the last scroll operation.
		   Explicitly setting point sets this to nil.  */
		Shelisp.Object point_before_scroll;

		/* Truename of the visited file, or nil.  */
		Shelisp.Object file_truename;

		/* Invisibility spec of this buffer.
		   t => any non-nil `invisible' property means invisible.
		   A list => `invisible' property means invisible
		   if it is memq in that list.  */
		Shelisp.Object invisibility_spec;

		/* This is the last window that was selected with this buffer in it,
		   or nil if that window no longer displays this buffer.  */
		Shelisp.Object last_selected_window;

		/* Incremented each time the buffer is displayed in a window.  */
		Shelisp.Object display_count;

		/* Widths of left and right marginal areas for windows displaying
		   this buffer.  */
		Shelisp.Object left_margin_cols, right_margin_cols;

		/* Widths of left and right fringe areas for windows displaying
		   this buffer.  */
		Shelisp.Object left_fringe_width, right_fringe_width;

		/* Non-nil means fringes are drawn outside display margins;
		   othersize draw them between margin areas and text.  */
		Shelisp.Object fringes_outside_margins;

		/* Width and type of scroll bar areas for windows displaying
		   this buffer.  */
		Shelisp.Object scroll_bar_width, vertical_scroll_bar_type;

		/* Non-nil means indicate lines not displaying text (in a style
		   like vi).  */
		private Shelisp.Object indicate_empty_lines = L.Qnil;
		[LispBuiltin]
		public static Shelisp.Object Vindicate_empty_lines {
			get { return Buffer.CurrentBuffer.indicate_empty_lines; }
			set { Buffer.CurrentBuffer.indicate_empty_lines = value; }
		}

		/* Non-nil means indicate buffer boundaries and scrolling.  */
		private Shelisp.Object indicate_buffer_boundaries = L.Qnil;
		[LispBuiltin]
		public static Shelisp.Object Vindicate_buffer_boundaries {
			get { return Buffer.CurrentBuffer.indicate_buffer_boundaries; }
			set { Buffer.CurrentBuffer.indicate_buffer_boundaries = value; }
		}

		/* Logical to physical fringe bitmap mappings.  */
		private Shelisp.Object fringe_indicator_alist = L.Qnil;
		[LispBuiltin]
		public static Shelisp.Object Vfringe_indicator_alist {
			get { return Buffer.CurrentBuffer.fringe_indicator_alist; }
			set { Buffer.CurrentBuffer.fringe_indicator_alist = value; }
		}

		/* Logical to physical cursor bitmap mappings.  */
		private Shelisp.Object fringe_cursor_alist = L.Qnil;
		[LispBuiltin]
		public static Shelisp.Object Vfringe_cursor_alist {
			get { return Buffer.CurrentBuffer.fringe_cursor_alist; }
			set { Buffer.CurrentBuffer.fringe_cursor_alist = value; }
		}


		/* Time stamp updated each time this buffer is displayed in a window.  */
		Shelisp.Object display_time;

		/* If scrolling the display because point is below the bottom of a
		   window showing this buffer, try to choose a window start so
		   that point ends up this number of lines from the top of the
		   window.  Nil means that scrolling method isn't used.  */
		private Shelisp.Object scroll_up_aggressively = L.Qnil;
		[LispBuiltin]
		public static Shelisp.Object Vscroll_up_aggressively {
			get { return Buffer.CurrentBuffer.scroll_up_aggressively; }
			set { Buffer.CurrentBuffer.scroll_up_aggressively = value; }
		}

		/* If scrolling the display because point is above the top of a
		   window showing this buffer, try to choose a window start so
		   that point ends up this number of lines from the bottom of the
		   window.  Nil means that scrolling method isn't used.  */
		private Shelisp.Object scroll_down_aggressively = L.Qnil;
		[LispBuiltin]
		public static Shelisp.Object Vscroll_down_aggressively {
			get { return Buffer.CurrentBuffer.scroll_down_aggressively; }
			set { Buffer.CurrentBuffer.scroll_down_aggressively = value; }
		}

		/* Desired cursor type in this buffer.  See the doc string of
		   per-buffer variable `cursor-type'.  */
		private Shelisp.Object cursor_type = L.Qnil;
		[LispBuiltin]
		public static Shelisp.Object Vcursor_type {
			get { return Buffer.CurrentBuffer.cursor_type; }
			set { Buffer.CurrentBuffer.cursor_type = value; }
		}

		/* An integer > 0 means put that number of pixels below text lines
		   in the display of this buffer.  */
		private Shelisp.Object extra_line_spacing = L.Qnil;
		[LispBuiltin ("line-spacing")]
		public static Shelisp.Object Vextra_line_spacing {
			get { return Buffer.CurrentBuffer.extra_line_spacing; }
			set { Buffer.CurrentBuffer.extra_line_spacing = value; }
		}

		/* *Cursor type to display in non-selected windows.
		   t means to use hollow box cursor.
		   See `cursor-type' for other values.  */
		private Shelisp.Object cursor_in_non_selected_windows = L.Qnil;
		[LispBuiltin]
		public static Shelisp.Object Vcursor_in_non_selected_windows {
			get { return Buffer.CurrentBuffer.cursor_in_non_selected_windows; }
			set { Buffer.CurrentBuffer.cursor_in_non_selected_windows = value; }
		}

		public Buffer next;

		public static Buffer all_buffers;
		public static Shelisp.Object Vbuffer_alist = L.Qnil;

		[LispBuiltin (DocString = @"Non-nil if Transient Mark mode is enabled.
See the command `transient-mark-mode' for a description of this minor mode.

Non-nil also enables highlighting of the region whenever the mark is active.
The variable `highlight-nonselected-windows' controls whether to highlight
all windows or just the selected window.

Lisp programs may give this variable certain special values:

- A value of `lambda' enables Transient Mark mode temporarily.
  It is disabled again after any subsequent action that would
  normally deactivate the mark (e.g. buffer modification).

- A value of (only . OLDVAL) enables Transient Mark mode
  temporarily.  After any subsequent point motion command that is
  not shift-translated, or any other action that would normally
  deactivate the mark (e.g. buffer modification), the value of
  `transient-mark-mode' is set to OLDVAL.")]
		public static Shelisp.Object Vtransient_mark_mode = L.Qnil;

	        [LispBuiltin (DocString = @"Non-nil means you can use the mark even when inactive.
This option makes a difference in Transient Mark mode.
When the option is non-nil, deactivation of the mark
turns off region highlighting, but commands that use the mark
behave as if the mark were still active.")]
		public static bool Vmark_even_if_inactive = true;

		[LispBuiltin (DocString = @"List of functions called with no args to query before killing a buffer.
The buffer being killed will be current while the functions are running.
If any of them returns nil, the buffer is not killed.")]
		public static Shelisp.Object Vkill_buffer_query_functions = L.Qnil;
	}

	public static class PerBuffer {
		/* Mode line element that controls format of mode line.  */
		public static Shelisp.Object mode_line_format {
			get { return Buffer.CurrentBuffer.mode_line_format; }
			set { Buffer.CurrentBuffer.mode_line_format = value; }
		}

		public static Shelisp.Object major_mode {
			get { return Buffer.CurrentBuffer.major_mode; }
			set { Buffer.CurrentBuffer.major_mode = value; }
		}
	}
}
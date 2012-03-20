using Shelisp;

namespace Shemacs.Editor {

	public class CodingSystem : Object {

		[LispBuiltin ("coding-system-p", MinArgs = 1)]
		public static Shelisp.Object Fcoding_system_p (L l, Shelisp.Object obj)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("read-non-nil-coding-system", MinArgs = 1)]
		public static Shelisp.Object Fread_non_nil_coding_system (L l, Shelisp.Object prompt)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("read-coding-system", MinArgs = 1)]
		public static Shelisp.Object Fread_coding_system (L l, Shelisp.Object prompt, Shelisp.Object default_coding_system)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("check-coding-system", MinArgs = 1)]
		public static Shelisp.Object Fcheck_coding_system (L l, Shelisp.Object coding_system)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("detect-coding-region", MinArgs = 2)]
		public static Shelisp.Object F_detect_coding_region (L l, Shelisp.Object start, Shelisp.Object end, Shelisp.Object highest)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("detect-coding-string", MinArgs = 1)]
		public static Shelisp.Object Fdetect_coding_string (L l, Shelisp.Object str, Shelisp.Object highest)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("find-coding-systems-region-internal", MinArgs = 2)]
		public static Shelisp.Object Ffind_coding_systems_region_internal (L l, Shelisp.Object start, Shelisp.Object end, Shelisp.Object exclude)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("unencodable-char-position", MinArgs = 3)]
		public static Shelisp.Object Funencodable_char_position (L l, Shelisp.Object start, Shelisp.Object end, Shelisp.Object coding_system, Shelisp.Object count, Shelisp.Object str)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("check-coding-systems-region", MinArgs = 3)]
		public static Shelisp.Object Fcheck_coding_systems_region (L l, Shelisp.Object start, Shelisp.Object end, Shelisp.Object coding_system_list)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("decode-coding-region", MinArgs = 3)]
		public static Shelisp.Object Fdecode_coding_region (L l, Shelisp.Object start, Shelisp.Object end, Shelisp.Object coding_system, Shelisp.Object destination)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("encode-coding-region", MinArgs = 3)]
		public static Shelisp.Object Fencoding_coding_region (L l, Shelisp.Object start, Shelisp.Object end, Shelisp.Object coding_system, Shelisp.Object destination)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("decode-coding-string", MinArgs = 2)]
		public static Shelisp.Object Fdecode_coding_string (L l, Shelisp.Object str, Shelisp.Object coding_system, Shelisp.Object nocopy, Shelisp.Object buffer)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("encode-coding-string", MinArgs = 2)]
		public static Shelisp.Object Fencode_coding_string (L l, Shelisp.Object str, Shelisp.Object coding_system, Shelisp.Object nocopy, Shelisp.Object buffer)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("decode-sjis-char", MinArgs = 1)]
		public static Shelisp.Object Fdecode_sjis_char (L l, Shelisp.Object code)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("encode-sjis-char", MinArgs = 1)]
		public static Shelisp.Object Fencode_sjis_char (L l, Shelisp.Object ch)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("decode-big5-char", MinArgs = 1)]
		public static Shelisp.Object Fdecode_big5_char (L l, Shelisp.Object code)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("encode-big5-char", MinArgs = 1)]
		public static Shelisp.Object Fencode_big5_char (L l, Shelisp.Object ch)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("set-terminal-coding-system-internal", MinArgs = 1)]
		public static Shelisp.Object Fset_terminal_coding_system_internal (L l, Shelisp.Object coding_system, Shelisp.Object terminal)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("set-safe-terminal-coding-system-internal", MinArgs = 1)]
		public static Shelisp.Object Fset_safe_terminal_coding_system_internal (L l, Shelisp.Object coding_system)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("terminal-coding-system", MinArgs = 0)]
		public static Shelisp.Object Fterminal_coding_system (L l, Shelisp.Object coding_system)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("set-keyboard-coding-system-internal", MinArgs = 1)]
		public static Shelisp.Object Fset_keyboard_coding_system_internal (L l, Shelisp.Object coding_system, Shelisp.Object terminal)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("keyboard-coding-system", MinArgs = 0)]
		public static Shelisp.Object Fkeyboard_coding_system (L l, Shelisp.Object terminal)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("find-operation-coding-system", MinArgs = 0)]
		public static Shelisp.Object Ffind_operation_coding_system (L l, params Shelisp.Object[] args)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("set-coding-system-priority", MinArgs = 0)]
		public static Shelisp.Object Fset_coding_system_priority (L l, params Shelisp.Object[] args)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("coding-system-priority-list", MinArgs = 0)]
		public static Shelisp.Object Fcoding_system_priority_list (L l, Shelisp.Object highestp)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("define-coding-system-internal", MinArgs = 0)]
		public static Shelisp.Object Fdefine_coding_system_internal (L l, params Shelisp.Object[] args)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("coding-system-put", MinArgs = 3)]
		public static Shelisp.Object Fcoding_system_put (L l, Shelisp.Object coding_system, Shelisp.Object prop, Shelisp.Object val)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("define-coding-system-alias", MinArgs = 2)]
		public static Shelisp.Object Fdefine_coding_system_alias (L l, Shelisp.Object alias, Shelisp.Object coding_system)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("coding-system-base", MinArgs = 1)]
		public static Shelisp.Object Fcoding_system_base (L l, Shelisp.Object coding_system)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("coding-system-plist", MinArgs = 1)]
		public static Shelisp.Object Fcoding_system_plist (L l, Shelisp.Object coding_system)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("coding-system-aliases", MinArgs = 1)]
		public static Shelisp.Object Fcoding_system_aliases (L l, Shelisp.Object coding_system)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("coding-system-eol-type", MinArgs = 1)]
		public static Shelisp.Object Fcoding_system_eol_type (L l, Shelisp.Object coding_system)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin ("coding-system-list")]
		public Shelisp.Object Vcoding_system_list = L.Qnil;

		[LispBuiltin ("coding-system-alist")]
		public Shelisp.Object Vcoding_system_alist = L.Qnil;

		[LispBuiltin ("coding-category-list")]
		public Shelisp.Object Vcoding_category_list = CodingSystem.create_category_list ();

		[LispBuiltin ("coding-system-for-write")]
		public Shelisp.Object Vcoding_system_for_write = L.Qnil;

		[LispBuiltin ("last-coding-system-used")]
		public Shelisp.Object Vlast_coding_system_used = L.Qnil;

		[LispBuiltin ("last-code-conversion-used")]
		public Shelisp.Object Vlast_code_conversion_used = L.Qnil;

		[LispBuiltin ("last-code-conversion-error")]
		public Shelisp.Object Vlast_code_conversion_error = L.Qnil;

		[LispBuiltin ("inhibit-eol-conversion")]
		public bool inhibit_eol_conversion = false;

		[LispBuiltin ("inherit-process-coding-system")]
		public bool inherit_process_coding_system = false;

		[LispBuiltin ("file-coding-system-alist")]
		public Shelisp.Object Vfile_coding_system_alist = L.Qnil;

		[LispBuiltin ("process-coding-system-alist")]
		public Shelisp.Object process_coding_system_alist = L.Qnil;

		[LispBuiltin ("network-coding-system-alist")]
		public Shelisp.Object network_coding_system_alist = L.Qnil;

		[LispBuiltin ("locale-coding-system")]
		public Shelisp.Object locale_coding_system = L.Qnil;

		[LispBuiltin ("eol-mnemonic-unix")]
		public Shelisp.Object eol_mnemonic_unix = (Shelisp.String)":";

		[LispBuiltin ("eol-mnemonic-dos")]
		public Shelisp.Object eol_mnemonic_dos = (Shelisp.String)"\\";

		[LispBuiltin ("eol-mnemonic-mac")]
		public Shelisp.Object eol_mnemonic_mac = (Shelisp.String)"/";

		[LispBuiltin ("eol-mnemonic-undecided")]
		public Shelisp.Object eol_mnemonic_undecided = (Shelisp.String)":";

		[LispBuiltin ("enable-character-translation")]
		public Shelisp.Object Venable_character_translation = L.Qt;

		[LispBuiltin ("standard-translation-table-for-decode")]
		public Shelisp.Object Vstandard_translation_table_for_decode = L.Qnil;

		[LispBuiltin ("standard-translation-table-for-encode")]
		public Shelisp.Object Vstandard_translation_table_for_encode = L.Qnil;

		[LispBuiltin ("charset-revision-table")]
		public Shelisp.Object Vcharset_revision_table = L.Qnil;

		[LispBuiltin ("default-process-coding-system")]
		public Shelisp.Object Vdefault_process_coding_system = L.Qnil;

		[LispBuiltin ("latin-extra-code-table")]
		public Shelisp.Object Vlatin_extra_code_table = new Vector (256, L.Qnil);

		[LispBuiltin ("select-safe-coding-system-function")]
		public Shelisp.Object Vselect_safe_coding_system_function = L.Qnil;

		[LispBuiltin ("coding-system-require-warning")]
		public bool coding_system_require_warning = false;

		[LispBuiltin ("inhibit-iso-escape-detection")]
		public bool inhibit_iso_escape_detection = false;

		[LispBuiltin ("inhibit-null-byte-detection")]
		public bool inhibit_null_byte_detection = false;

		[LispBuiltin ("translation-table-for-input")]
		public Shelisp.Object Vtranslation_table_for_input = L.Qnil;

#if not_ported
  {
    Lisp_Object args[coding_arg_max];
    Lisp_Object plist[16];
    int i;

    for (i = 0; i < coding_arg_max; i++)
      args[i] = Qnil;

    plist[0] = intern_c_string (":name");
    plist[1] = args[coding_arg_name] = Qno_conversion;
    plist[2] = intern_c_string (":mnemonic");
    plist[3] = args[coding_arg_mnemonic] = make_number ('=');
    plist[4] = intern_c_string (":coding-type");
    plist[5] = args[coding_arg_coding_type] = Qraw_text;
    plist[6] = intern_c_string (":ascii-compatible-p");
    plist[7] = args[coding_arg_ascii_compatible_p] = Qt;
    plist[8] = intern_c_string (":default-char");
    plist[9] = args[coding_arg_default_char] = make_number (0);
    plist[10] = intern_c_string (":for-unibyte");
    plist[11] = args[coding_arg_for_unibyte] = Qt;
    plist[12] = intern_c_string (":docstring");
    plist[13] = make_pure_c_string ("Do no conversion.\n\
\n\
When you visit a file with this coding, the file is read into a\n\
unibyte buffer as is, thus each byte of a file is treated as a\n\
character.");
    plist[14] = intern_c_string (":eol-type");
    plist[15] = args[coding_arg_eol_type] = Qunix;
    args[coding_arg_plist] = Flist (16, plist);
    Fdefine_coding_system_internal (coding_arg_max, args);

    plist[1] = args[coding_arg_name] = Qundecided;
    plist[3] = args[coding_arg_mnemonic] = make_number ('-');
    plist[5] = args[coding_arg_coding_type] = Qundecided;
    /* This is already set.
       plist[7] = args[coding_arg_ascii_compatible_p] = Qt; */
    plist[8] = intern_c_string (":charset-list");
    plist[9] = args[coding_arg_charset_list] = Fcons (Qascii, Qnil);
    plist[11] = args[coding_arg_for_unibyte] = Qnil;
    plist[13] = make_pure_c_string ("No conversion on encoding, automatic conversion on decoding.");
    plist[15] = args[coding_arg_eol_type] = Qnil;
    args[coding_arg_plist] = Flist (16, plist);
    Fdefine_coding_system_internal (coding_arg_max, args);
  }

  setup_coding_system (Qno_conversion, &safe_terminal_coding);

  {
    int i;

    for (i = 0; i < coding_category_max; i++)
      Fset (AREF (Vcoding_category_table, i), Qno_conversion);
  }
#if defined (DOS_NT)
  system_eol_type = Qdos;
#else
  system_eol_type = Qunix;
#endif
  staticpro (&system_eol_type);
#endif
		static Shelisp.Object create_category_list ()
		{
			int i;
			Shelisp.Object rv = L.Qnil;

			for (i = coding_category_table.Length - 1; i >= 0; i--)
				rv = new List (coding_category_table[i], rv);
			return rv;
		}

		static Vector coding_category_table;

		static CodingSystem ()
		{
			coding_category_table = new Vector ((int)coding_category.max, L.Qnil);
			/* Followings are target of code detection.  */
			coding_category_table [(int)coding_category.iso_7] = L.intern ("coding-category-iso-7");
			coding_category_table [(int)coding_category.iso_7_tight] = L.intern ("coding-category-iso-7-tight");
			coding_category_table [(int)coding_category.iso_8_1] = L.intern ("coding-category-iso-8-1");
			coding_category_table [(int)coding_category.iso_8_2] = L.intern ("coding-category-iso-8-2");
			coding_category_table [(int)coding_category.iso_7_else] = L.intern ("coding-category-iso-7-else");
			coding_category_table [(int)coding_category.iso_8_else] = L.intern ("coding-category-iso-8-else");
			coding_category_table [(int)coding_category.utf_8_auto] = L.intern ("coding-category-utf-8-auto");
			coding_category_table [(int)coding_category.utf_8_nosig] = L.intern ("coding-category-utf-8");
			coding_category_table [(int)coding_category.utf_8_sig] = L.intern ("coding-category-utf-8-sig");
			coding_category_table [(int)coding_category.utf_16_be] = L.intern ("coding-category-utf-16-be");
			coding_category_table [(int)coding_category.utf_16_auto] = L.intern ("coding-category-utf-16-auto");
			coding_category_table [(int)coding_category.utf_16_le] = L.intern ("coding-category-utf-16-le");
			coding_category_table [(int)coding_category.utf_16_be_nosig] = L.intern ("coding-category-utf-16-be-nosig");
			coding_category_table [(int)coding_category.utf_16_le_nosig] = L.intern ("coding-category-utf-16-le-nosig");
			coding_category_table [(int)coding_category.charset] = L.intern ("coding-category-charset");
			coding_category_table [(int)coding_category.sjis] = L.intern ("coding-category-sjis");
			coding_category_table [(int)coding_category.big5] = L.intern ("coding-category-big5");
			coding_category_table [(int)coding_category.ccl] = L.intern ("coding-category-ccl");
			coding_category_table [(int)coding_category.emacs_mule] = L.intern ("coding-category-emacs-mule");
			/* Followings are NOT target of code detection.  */
			coding_category_table [(int)coding_category.raw_text] = L.intern ("coding-category-raw-text");
			coding_category_table [(int)coding_category.undecided] = L.intern ("coding-category-undecided");
		}

		enum coding_category {
			iso_7,
			iso_7_tight,
			iso_8_1,
			iso_8_2,
			iso_7_else,
			iso_8_else,
			utf_8_auto,
			utf_8_nosig,
			utf_8_sig,
			utf_16_auto,
			utf_16_be,
			utf_16_le,
			utf_16_be_nosig,
			utf_16_le_nosig,
			charset,
			sjis,
			big5,
			ccl,
			emacs_mule,
			/* All above are targets of code detection.  */
			raw_text,
			undecided,
			max
		};
	}

}

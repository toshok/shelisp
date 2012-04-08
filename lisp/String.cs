using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Shelisp {
	public class String : Array {
		public String (string s)
		{
			this.native_string = new StringBuilder(s);
		}

		public override bool LispEqual (Shelisp.Object other)
		{
			if (!(other is String))
				return false;
			return this.native_string.ToString() == ((String)other).native_string.ToString();
		}

		public StringBuilder native_string;

		public override int Length {
			get { return native_string.Length; }
		}

		public override Shelisp.Object this[int index]
		{
			get { return (Number)(int)native_string[index]; }
			set {
				if (Number.IsInt(value))
					throw new WrongTypeArgumentException ("integer", value ?? L.Qnil);
				native_string[index] = (char)Number.ToInt(value);
			}
		}

		public override IEnumerator<Shelisp.Object> GetEnumerator ()
		{
			for (int i = 0; i < native_string.Length; i ++)
				yield return (int)native_string[i];
		}

		public override int GetHashCode ()
		{
			return native_string.ToString().GetHashCode();
		}

		public override string ToString (string format_type)
		{
			switch (format_type) {
			case "princ":
				return native_string.ToString();
			case "prin1":
				return string.Concat ("\"", native_string.ToString(), "\"");
			default:
				throw new NotSupportedException (string.Format ("unsupported format type `{0}'", format_type));
			}
		}

		public static implicit operator System.String (String str)
		{
			return str.native_string.ToString();
		}

		[LispBuiltin]
		public static Shelisp.Object Fstring_match(L l, Shelisp.Object regex, Shelisp.Object str, [LispOptional] Shelisp.Object start)
		{
			if (!(regex is Shelisp.String))
				throw new WrongTypeArgumentException ("stringp", regex);
			if (!(str is Shelisp.String))
				throw new WrongTypeArgumentException ("stringp", str);
			if (!L.NILP(start) && !Number.IsInt(start))
				throw new WrongTypeArgumentException ("integerp", start);

			string regex_s = (string)(String)regex;
			string str_s = (string)(String)str;
			int start_i = L.NILP(start) ? 0 : (int)((Shelisp.Number)start).boxed;

			regex_s = regex_s.Replace ("\\(", "OMGOPENPAREN").Replace ("(", "\\(").Replace ("OMGOPENPAREN", "(");
			regex_s = regex_s.Replace ("\\)", "OMGCLOSEPAREN").Replace (")", "\\)").Replace ("OMGCLOSEPAREN", ")");
			regex_s = regex_s.Replace ("\\'", "$");
			var re = new Regex (regex_s);

			var match = re.Match (str_s, start_i);

			if (match.Success == false) {
				// do we need to clear out L.match_data?
				return L.Qnil;
			}

			int[] match_data = new int[match.Groups.Count * 2];
			for (int i = 0; i < match.Groups.Count; i ++) {
				match_data[i*2] = match.Groups[i].Index;
				match_data[i*2 + 1] = match.Groups[i].Index + match.Groups[i].Length;
			}

			L.match_data = match_data;

			return new Shelisp.Number (match.Groups[0].Index);
		}

		[LispBuiltin]
		public static Shelisp.Object Fmatch_data (L l)
		{
			if (L.match_data == null)
				return L.Qnil;

			return L.int_array_to_list (L.match_data);
		}

		[LispBuiltin (DocString = @"Set internal data on last search match from elements of LIST.
LIST should have been created by calling `match-data' previously.

If optional arg RESEAT is non-nil, make markers on LIST point nowhere.")]
		public static Shelisp.Object Fset_match_data (L l, Shelisp.Object data, Shelisp.Object reseat)
		{
			if (!L.LISTP(data))
				throw new WrongTypeArgumentException ("listp", data);

			if (L.NILP (data))
				L.match_data = null;
			
			List data_l = (List)data;
			L.match_data = new int[data_l.Length];
			int i = 0;
			foreach (var item in data_l)
				L.match_data[i++] = (int)(Number)item;

			return data;
		}

		[LispBuiltin]
		public static Shelisp.Object Fmatch_end (L l, Shelisp.Object idx, [LispOptional] Shelisp.Object str)
		{
			if (!L.NILP(idx) && !Number.IsInt(idx))
				throw new WrongTypeArgumentException ("integerp", idx);

			int idx_i = (int)((Shelisp.Number)idx).boxed;

			if (L.match_data == null)
				return L.Qnil;

			return idx_i > L.match_data.Length/2 ? L.Qnil : new Shelisp.Number (L.match_data[idx_i * 2 + 1]);
		}

		[LispBuiltin]
		public static Shelisp.Object Fmatch_beginning (L l, Shelisp.Object idx, [LispOptional] Shelisp.Object str)
		{
			if (!L.NILP(idx) && !Number.IsInt(idx))
				throw new WrongTypeArgumentException ("integerp", idx);

			int idx_i = (int)((Shelisp.Number)idx).boxed;

			if (L.match_data == null)
				return L.Qnil;

			return idx_i > L.match_data.Length/2 ? L.Qnil : new Shelisp.Number (L.match_data[idx_i * 2]);
		}

		[LispBuiltin (DocString = @"Replace text matched by last search with NEWTEXT.
Leave point at the end of the replacement text.

If second arg FIXEDCASE is non-nil, do not alter case of replacement text.
Otherwise maybe capitalize the whole text, or maybe just word initials,
based on the replaced text.
If the replaced text has only capital letters
and has at least one multiletter word, convert NEWTEXT to all caps.
Otherwise if all words are capitalized in the replaced text,
capitalize each word in NEWTEXT.

If third arg LITERAL is non-nil, insert NEWTEXT literally.
Otherwise treat `\\' as special:
  `\\&' in NEWTEXT means substitute original matched text.
  `\\N' means substitute what matched the Nth `\\(...\\)'.
       If Nth parens didn't match, substitute nothing.
  `\\\\' means insert one `\\'.
Case conversion does not apply to these substitutions.

FIXEDCASE and LITERAL are optional arguments.

The optional fourth argument STRING can be a string to modify.
This is meaningful when the previous match was done against STRING,
using `string-match'.  When used this way, `replace-match'
creates and returns a new string made by copying STRING and replacing
the part of STRING that was matched.

The optional fifth argument SUBEXP specifies a subexpression;
it says to replace just that subexpression with NEWTEXT,
rather than replacing the entire matched text.
This is, in a vague sense, the inverse of using `\\N' in NEWTEXT;
`\\N' copies subexp N into NEWTEXT, but using N as SUBEXP puts
NEWTEXT in place of subexp N.
This is useful only after a regular expression search or match,
since only regular expressions have distinguished subexpressions.")]
		public static Shelisp.Object Freplace_match (L l, Shelisp.Object newtext, [LispOptional] Shelisp.Object fixedcase, Shelisp.Object literal, Shelisp.Object str, Shelisp.Object subexp)
		{
			if (!L.NILP (str)) {
				int start = L.match_data[0];
				int end = L.match_data[1];
				string str_s = (string)(Shelisp.String)str;

				return (Shelisp.String)System.String.Concat (str_s.Substring (0, start), newtext.ToString("princ"), str_s.Substring (end));
			}
			else {
				Console.WriteLine ("replace-match not implemented"); return str;
			}
		}

		[LispBuiltin]
		public static Shelisp.Object Fsubstring (L l, Shelisp.Object str, Shelisp.Object start, [LispOptional] Shelisp.Object end)
		{
			if (!(str is Shelisp.String))
				throw new WrongTypeArgumentException ("stringp", str);
			if (!Number.IsInt(start))
				throw new WrongTypeArgumentException ("integerp", start);
			if (!L.NILP(end) && !Number.IsInt(end))
				throw new WrongTypeArgumentException ("integerp", end);

			string str_s = (string)(Shelisp.String)str;
			int start_i = Number.ToInt(start);
			int end_i = L.NILP(end) ? str_s.Length : Number.ToInt(end);

			if (start_i < 0)
				start_i += str_s.Length;
			if (end_i < 0)
				end_i += str_s.Length;

			return (Shelisp.String)str_s.Substring (start_i, end_i - start_i);
		}

		[LispBuiltin (DocString = @"Return a substring of STRING, without text properties.
It starts at index FROM and ends before TO.
TO may be nil or omitted; then the substring runs to the end of STRING.
If FROM is nil or omitted, the substring starts at the beginning of STRING.
If FROM or TO is negative, it counts from the end.

With one argument, just copy STRING without its properties.")]
		public static Shelisp.Object Fsubstring_no_properties (L l, Shelisp.Object str, [LispOptional] Shelisp.Object from, Shelisp.Object to)
		{
			if (!(str is Shelisp.String))
				throw new WrongTypeArgumentException ("stringp", str);
			if (!L.NILP(from) && !Number.IsInt(from))
				throw new WrongTypeArgumentException ("integerp", from);
			if (!L.NILP(to) && !Number.IsInt(to))
				throw new WrongTypeArgumentException ("integerp", to);

			string str_s = (string)(Shelisp.String)str;
			int from_i = L.NILP(from) ? 0 : Number.ToInt(from);
			int to_i = L.NILP(to) ? str_s.Length : Number.ToInt(to);

			if (from_i < 0)
				from_i += str_s.Length;
			if (to_i < 0)
				to_i += str_s.Length;

			return (Shelisp.String)str_s.Substring (from_i, to_i - from_i);
		}

		[LispBuiltin]
		public static Shelisp.Object Fstring_to_number(L l, Shelisp.Object str)
		{
			if (!(str is Shelisp.String))
				throw new WrongTypeArgumentException ("stringp", str);

			string s = (string)(String)str;

			int i;
			if (Int32.TryParse (s, out i))
				return new Shelisp.Number(i);
			float f;
			if (Single.TryParse (s, out f))
				return new Shelisp.Number(f);

			throw new Exception (string.Format ("failed to parse string '{0}'", s));
		}

		[LispBuiltin]
		public static Shelisp.Object Fstring_to_multibyte (L l, Shelisp.Object str)
		{
			Console.WriteLine ("string-to-multibyte not implemented");
			return str;
		}

		[LispBuiltin]
		public static Shelisp.Object Fupcase(L l, Shelisp.Object str)
		{
			if (!(str is Shelisp.String))
				throw new WrongTypeArgumentException ("stringp", str);

			string s = (string)(String)str;

			return (Shelisp.String)s.ToUpper();
		}

		[LispBuiltin]
		public static Shelisp.Object Fstring_equal(L l, Shelisp.Object str1, Shelisp.Object str2)
		{
			if (!(str1 is Shelisp.String && str2 is Shelisp.String))
				return L.Qnil;

			return (string)(Shelisp.String)str1 == (string)(Shelisp.String)str2 ? L.Qt : L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fconcat(L l, params Shelisp.Object[] seqs)
		{
			StringBuilder sb = new StringBuilder ();
			foreach (var s in seqs) {
				if (L.NILP (s))
					continue;
				if (!(s is Sequence))
					throw new WrongTypeArgumentException ("sequencep", s);
				Sequence seq = (Sequence)s;
				if (seq is String)
					sb.Append ((string)(Shelisp.String)seq);
				else {
					foreach (var el in seq) {
						if (Number.IsInt (el))
							sb.Append ((char)Number.ToInt(el));
						else
							sb.Append (el.ToString("prin1"));
					}
				}
			}

			return (Shelisp.String)sb.ToString();
		}

		[LispBuiltin]
		public static Shelisp.Object Fstringp(L l, Shelisp.Object str)
		{
			return str is Shelisp.String ? L.Qt : L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fmultibyte_string_p(L l, Shelisp.Object str)
		{
			// XXX
			return str is Shelisp.String ? L.Qt : L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fpropertize(L l, Shelisp.Object str, params Shelisp.Object[] propvals)
		{
			Console.WriteLine ("propertize not implemented");
			return str;
		}

		[LispBuiltin (DocString = "Return a regexp string which matches exactly STRING and nothing else.")]
		public static Shelisp.Object Fregexp_quote (L l, Shelisp.Object str)
		{
			if (!(str is Shelisp.String))
			    throw new WrongTypeArgumentException ("stringp", str);

			StringBuilder sb = new StringBuilder ();
			string str_s = (string)(Shelisp.String)str;
			for (int i = 0; i < str_s.Length; i ++) {
				char str_i = str_s[i];
				if (str_i == '['
				    || str_i == '*' || str_i == '.' || str_i == '\\'
				    || str_i == '?' || str_i == '+'
				    || str_i == '^' || str_i == '$')
					sb.Append ('\\');
				sb.Append (str_i);
			}

			return (Shelisp.String)sb.ToString();
		}

		[LispBuiltin]
		public static Shelisp.Object Fre_search_forward (L l, Shelisp.Object regexp, [LispOptional] Shelisp.Object bound, Shelisp.Object noerror, Shelisp.Object count)
		{
			Console.WriteLine ("re-search-forward not implemented");
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fre_search_backward (L l, Shelisp.Object regexp, [LispOptional] Shelisp.Object bound, Shelisp.Object noerror, Shelisp.Object count)
		{
			Console.WriteLine ("re-search-backward not implemented");
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fchar_to_string (L l, Shelisp.Object ch)
		{
			string str = new string ((char)(int)(Shelisp.Number)ch, 1);
			
			return (Shelisp.String)str;
		}


		[LispBuiltin (DocString = @"Convert argument to capitalized form and return that.
This means that each word's first character is upper case
and the rest is lower case.
The argument may be a character or string.  The result has the same type.
The argument object is not altered--the value is a copy.")]
		public static Shelisp.Object Fcapitalize (L l, Shelisp.Object obj)
		{
			if (obj is Shelisp.String) {
				StringBuilder sb = new StringBuilder ((string)(Shelisp.String)obj);

				bool need_capitalization = true;
				for (int i = 0; i < sb.Length; i ++) {
					if (Char.IsLetter (sb[i])) {
						if (need_capitalization) {
							sb[i] = Char.ToUpper(sb[i]);
							need_capitalization = false;
						}
					}
					else {
						need_capitalization = true;
					}

				}

				return (Shelisp.String)sb.ToString();
			}
			else {
				Console.WriteLine ("unimplemented non-string version of capitalize");
				return obj;
			}
		}

		[LispBuiltin (DocString = @"Return t if first arg string is less than second in lexicographic order.
Case is significant.
Symbols are also allowed; their print names are used instead.")]
		public static Shelisp.Object Fstring_lessp (L l, Shelisp.Object s1, Shelisp.Object s2)
		{
			string s1_s, s2_s;

			if (s1 is Symbol)
				s1_s = ((Symbol)s1).name;
			else {
				if (!(s1 is String))
					throw new WrongTypeArgumentException ("stringp", s1);
				s1_s = (string)(Shelisp.String)s1;
			}

			if (s2 is Symbol)
				s2_s = ((Symbol)s2).name;
			else {
				if (!(s2 is String))
					throw new WrongTypeArgumentException ("stringp", s2);
				s2_s = (string)(Shelisp.String)s2;
			}

			return s1_s.CompareTo(s2_s) < 0 ? L.Qt : L.Qnil;
		}
	}

}
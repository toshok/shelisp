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

		public StringBuilder native_string;

		public override int Length {
			get { return native_string.Length; }
		}

		public override Shelisp.Object this[int index]
		{
			get { return native_string[index]; }
			set { /* XXX type check */ native_string[index] = (char)(int)(Number)value; }
		}

		public override IEnumerator<Shelisp.Object> GetEnumerator ()
		{
			for (int i = 0; i < native_string.Length; i ++)
				yield return native_string[i];
		}

		public override string ToString ()
		{
			return string.Concat ("\"", native_string.ToString(), "\"");
		}

		public static implicit operator System.String (String str)
		{
			return str.native_string.ToString();
		}

		[LispBuiltin ("string-match", MinArgs = 2)]
		public static Shelisp.Object Fstring_match(L l, Shelisp.Object regex, Shelisp.Object str, Shelisp.Object start)
		{
			if (!(regex is Shelisp.String))
				throw new WrongTypeArgumentException ("stringp", regex);
			if (!(str is Shelisp.String))
				throw new WrongTypeArgumentException ("stringp", str);
			if ((start != null) && !(start is Shelisp.Number && ((Shelisp.Number)start).boxed is int))
				throw new WrongTypeArgumentException ("integerp", start);

			string regex_s = (string)(String)regex;
			string str_s = (string)(String)str;
			int start_i = start == null ? 0 : (int)((Shelisp.Number)start).boxed;


			Console.WriteLine ("before = {0}", regex_s);

			regex_s = regex_s.Replace ("\\(", "OMGOPENPAREN").Replace ("(", "\\(").Replace ("OMGOPENPAREN", "(");
			regex_s = regex_s.Replace ("\\)", "OMGCLOSEPAREN").Replace (")", "\\)").Replace ("OMGCLOSEPAREN", ")");

			Console.WriteLine ("after {0}", regex_s);

			var re = new Regex (regex_s);

			var match = re.Match (str_s, start_i);

			L.current_match = match;

			if (match == null || match.Groups.Count == 0) {
				return L.Qnil;
			}
			else {
				return new Shelisp.Number (match.Groups[0].Index);
			}
		}

		[LispBuiltin ("match-end", MinArgs = 1)]
		public static Shelisp.Object Fmatch_end (L l, Shelisp.Object idx, Shelisp.Object str)
		{
			if (idx != null && !(idx is Shelisp.Number && ((Shelisp.Number)idx).boxed is int))
				throw new WrongTypeArgumentException ("integerp", idx);

			int idx_i = (int)((Shelisp.Number)idx).boxed;

			var match = L.current_match;
			if (match == null)
				return L.Qnil;

			return idx_i >= match.Groups.Count ? L.Qnil : new Shelisp.Number (match.Groups[idx_i].Index + match.Groups[idx_i].Length);
		}

		[LispBuiltin ("match-beginning", MinArgs = 1)]
		public static Shelisp.Object Fmatch_beginning (L l, Shelisp.Object idx, Shelisp.Object str)
		{
			if (idx != null && !(idx is Shelisp.Number && ((Shelisp.Number)idx).boxed is int))
				throw new WrongTypeArgumentException ("integerp", idx);

			int idx_i = (int)((Shelisp.Number)idx).boxed;

			var match = L.current_match;
			if (match == null)
				return L.Qnil;

			return idx_i >= match.Groups.Count ? L.Qnil : new Shelisp.Number (match.Groups[idx_i].Index);
		}

		[LispBuiltin ("substring", MinArgs = 2)]
		public static Shelisp.Object Fsubstring (L l, Shelisp.Object str, Shelisp.Object start, Shelisp.Object end)
		{
			if (!(str is Shelisp.String))
				throw new WrongTypeArgumentException ("stringp", str);
			if (!(start is Shelisp.Number && ((Shelisp.Number)start).boxed is int))
				throw new WrongTypeArgumentException ("integerp", start);
			if (end != null && !(end is Shelisp.Number && ((Shelisp.Number)end).boxed is int))
				throw new WrongTypeArgumentException ("integerp", end);

			string str_s = (string)(Shelisp.String)str;
			int start_i = (int)((Shelisp.Number)start).boxed;
			int end_i = end == null ? -1 : (int)((Shelisp.Number)end).boxed;

			return (Shelisp.String)str_s.Substring (start_i, end_i - start_i);
		}

		[LispBuiltin ("string-to-number", MinArgs = 1)]
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

		[LispBuiltin ("string-equal", MinArgs = 2)]
		public static Shelisp.Object Fstring_equal(L l, Shelisp.Object str1, Shelisp.Object str2)
		{
			if (!(str1 is Shelisp.String && str2 is Shelisp.String))
				return L.Qnil;

			return (string)(Shelisp.String)str1 == (string)(Shelisp.String)str2 ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("concat")]
		public static Shelisp.Object Fconcat(L l, params Shelisp.Object[] seqs)
		{
			// XXX
			return seqs[0];
		}
	}

}
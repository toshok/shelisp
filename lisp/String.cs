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
			set { /* XXX type check */ native_string[index] = (char)(int)(Number)value; }
		}

		public override IEnumerator<Shelisp.Object> GetEnumerator ()
		{
			for (int i = 0; i < native_string.Length; i ++)
				yield return native_string[i];
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

			var re = new Regex (regex_s);

			var match = re.Match (str_s, start_i);

			int[] match_data = new int[match.Groups.Count * 2];
			for (int i = 0; i < match.Groups.Count; i ++) {
				match_data[i*2] = match.Groups[i].Index;
				match_data[i*2 + 1] = match.Groups[i].Index + match.Groups[i].Length;
			}

			L.match_data = match_data;

			if (match == null || match.Groups.Count == 0) {
				return L.Qnil;
			}
			else {
				return new Shelisp.Number (match.Groups[0].Index);
			}
		}

		[LispBuiltin]
		public static Shelisp.Object Fmatch_data (L l)
		{
			if (L.match_data == null)
				return L.Qnil;

			return L.int_array_to_list (L.match_data);
		}

		[LispBuiltin]
		public static Shelisp.Object Fset_match_data (L l, Shelisp.Object data)
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
			int start_i = (int)((Shelisp.Number)start).boxed;
			int end_i = L.NILP(end) ? -1 : (int)((Shelisp.Number)end).boxed;

			return (Shelisp.String)str_s.Substring (start_i, end_i - start_i);
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
		public static Shelisp.Object Fstring_equal(L l, Shelisp.Object str1, Shelisp.Object str2)
		{
			if (!(str1 is Shelisp.String && str2 is Shelisp.String))
				return L.Qnil;

			return (string)(Shelisp.String)str1 == (string)(Shelisp.String)str2 ? L.Qt : L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fconcat(L l, params Shelisp.Object[] seqs)
		{
			// XXX
			return seqs[0];
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
			// XXX this doesn't just apply to strings..
			return obj;
		}
	}

}
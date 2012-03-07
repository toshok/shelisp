using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lisp {
	public static class Reader {
		public static Lisp.Object Read (string s)
		{
			return Read (new StringReader (s));
		}

		public static Lisp.Object Read (TextReader s, char valid_end = (char)0)
		{
			start:
			char ch;

			do {
				ch = (char)s.Peek();
				if (Char.IsWhiteSpace(ch))
					s.Read();
			} while (Char.IsWhiteSpace (ch));

			if (Char.IsDigit(ch))
				return ReadNumber (s);
			else if (ch == '(')
				return ReadList (s);
			else if (ch == '[')
				return ReadVector (s);
			else if (SymbolStartChar (ch))
				return ReadSymbol (s);
			else if (ch == '\'') {
				s.Read();
				return new Quote (Read(s));
			}
			else if (ch == '"') {
				return ReadString(s);
			}
			else if (ch == ';') {
				s.ReadLine();
				goto start;
			}
			else if (ch == valid_end) {
				s.Read();// consume the sentinel
				return null; // a special sentinel, check ReadList
			}
			else
				throw new Exception (string.Format ("unrecognized character '{0}' in input stream", ch));
		}

		private static bool SymbolStartChar(char ch)
		{
			return (Char.IsLetter (ch) || ch == '_');
		}

		private static bool SymbolChar(char ch)
		{
			return (Char.IsLetter (ch) || Char.IsDigit(ch) || ch == '_' || ch == '-' || ch == '?');
		}

		private static Lisp.String ReadString (TextReader s)
		{
			StringBuilder sb = new StringBuilder ();

			// consume the initial "
			s.Read();

			while (s.Peek() != '"') {
				sb.Append ((char)s.Read());
			}
			s.Read(); // make sure to consume the last "

			return new String (sb.ToString());
		}

		private static Lisp.List ReadList (TextReader s)
		{
			Debug.Print ("ReadList>");
			// consume the (
			s.Read();

			List<Lisp.Object> objs = new List<Lisp.Object>();
			Lisp.Object obj;
			while ((obj = Read (s, ')')) != null) {
				Debug.Print ("+ {0}", obj);
				objs.Add (obj);
			}

			var rv = L.make_list (objs.ToArray());
			Debug.Print ("ReadList returning {0}", rv);
			return rv;
		}

		private static Lisp.Vector ReadVector (TextReader s)
		{
			Debug.Print ("ReadVector>");
			// consume the (
			s.Read();

			List<Lisp.Object> objs = new List<Lisp.Object>();
			Lisp.Object obj;
			while ((obj = Read (s, ']')) != null) {
				Debug.Print ("+ {0}", obj);
				objs.Add (obj);
			}

			var rv = new Vector (objs.ToArray());
			Debug.Print ("ReadList returning {0}", rv);
			return rv;
		}

		private static Lisp.Number ReadNumber (TextReader s)
		{
			StringBuilder sb = new StringBuilder ();

			bool valid = true;
			do {
				if (valid)
					sb.Append ((char)s.Read());
				valid = Char.IsDigit ((char)s.Peek());
			} while (valid);

			return new Number (Int32.Parse (sb.ToString()));
		}

		private static Lisp.Symbol ReadSymbol (TextReader s)
		{
			StringBuilder sb = new StringBuilder ();
			bool valid = true;

			do {
				if (valid)
					sb.Append ((char)s.Read());
				valid = SymbolChar ((char)s.Peek());
			} while (valid);

			return L.intern (sb.ToString());
		}
	}
}
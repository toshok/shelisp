using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shelisp {
	public static class Reader {
		class StringPositionReader : StringReader {
			public StringPositionReader (string str)
				: base (str)
			{
				Position = 0;
			}

			public override int Read ()
			{
				Position++;
				return base.Read ();
			}

			public override int Read (char [] buffer, int index, int count)
			{
				int read = base.Read (buffer, index, count);
				Position += read;
				return read;
			}

			public override int ReadBlock (char [] buffer, int index, int count)
			{
				int read = base.ReadBlock (buffer, index, count);
				Position += read;
				return read;
			}

			public override string ReadLine ()
			{
				string line = base.ReadLine ();
				Position += line.Length; /* +1 for \n? */
				return line;
			}

			public override string ReadToEnd ()
			{
				string rest = base.ReadToEnd ();
				Position += rest.Length;
				return rest;
			}

			public int Position { get; private set; }

		}

#if false
		public static Shelisp.Object Read (string s)
		{
			int unused;
			return Read (s, out unused);
		}

		public static Shelisp.Object Read (string s, out int end_position)
		{
			var sr = new StringPositionReader (s);
			var obj = Read (sr);
			end_position = sr.Position;
			return obj;
		}
#endif

		public static Shelisp.Object Read (StreamReader s, char valid_end = (char)0)
		{
			char ch;
			bool escaped = false;

			StringBuilder sb = new StringBuilder ();

			start:
			if (s.EndOfStream) {
				if (escaped)
					throw new Exception ("unexpected end of file");
				else if (sb.Length > 0)
					return ReadSymbolLikeThing (sb.ToString());
				else {
					return null;
					//throw new Exception ("end of stream while reading");
				}
					
			}

			if (escaped) {
				// XXX unicode?  control characters?  what else?
				sb.Append ((char)s.Read());
				escaped = false;
				goto start;
			}
			else {
				ch = (char)s.Peek();
				if (Char.IsWhiteSpace (ch)) {
					if (sb.Length > 0) {
						return ReadSymbolLikeThing (sb.ToString());
					}
					else {
						do {
							s.Read();
							ch = (char)s.Peek();
						} while (Char.IsWhiteSpace (ch));

						goto start;
					}
				}
				if (ch == '?') {
					s.Read();
					return ReadCharacterLiteral (s);
				}
				else if (ch == '\\') {
					s.Read();
					escaped = true;
					goto start;
				}
				if (ch == '"') {
					return ReadString (s);
				}
				else if (ch == '(') {
					if (sb.Length > 0) {
						return ReadSymbolLikeThing (sb.ToString());
					}
					else {
						return ReadList (s);
					}
				}
				else if (ch == '[') {
					if (sb.Length > 0) {
						return ReadSymbolLikeThing (sb.ToString());
					}
					else {
						return ReadVector (s);
					}
				}
				else if (ch == valid_end) {
					if (sb.Length > 0) {
						// we return the item first.  our caller will call us again at which point we'll end up in the else branch here.
						return ReadSymbolLikeThing (sb.ToString());
					}
					else {
						s.Read(); // consume the end character
						return null; // a special sentinel, check ReadList/ReadVector
					}
				}
				else if (ch == ';') {
					// comment, ignore the rest of the line and continue reading
					if (sb.Length > 0) {
						return ReadSymbolLikeThing (sb.ToString());
					}
					else {
						s.ReadLine();
						goto start;
					}
				}
				else if (ch == '\'') {
					if (sb.Length > 0) {
						return ReadSymbolLikeThing (sb.ToString());
					}
					else {
						s.Read(); // consume the quote and recurse
						return new Quote (Read(s, valid_end));
					}
				}
				else {
					sb.Append ((char)s.Read());
					goto start;
				}
			}
		}

		private static Shelisp.Object ReadSymbolLikeThing (string contents)
		{
			int i;
			if (Int32.TryParse (contents, out i))
				return new Shelisp.Number (i);

			float f;
			if (Single.TryParse (contents, out f))
				return new Shelisp.Number (f);

			return L.intern (contents);
		}

		static int ReturnCharOrThrow (TextReader s, int value)
		{
			Console.WriteLine ("ReturnCharOrThrow ({0})", value);
			// check if the next character in our reader is valid ending punctuation (whitespace or ')'.. anything else?)
			char ch = (char)s.Peek();
			if (Char.IsWhiteSpace(ch) || ch == ')' || ch == ']')
				return value;
			
			Console.WriteLine ("next character is '{0}'", ch);
			throw new Exception ("(invalid-read-syntax ?)");
		}

		static int ReadHexNumber (TextReader s)
		{
			string hex = "0123456789ABCDEF";
			int value = 0;
			int hex_digit;

			while ((hex_digit = hex.IndexOf (Char.ToUpper ((char)s.Peek()))) != -1) {
				s.Read();
				value = (value << 4) + hex_digit;
			}
			return value;
		}

		static int ReadOctalNumber (TextReader s, int value)
		{
			while (true) {
				char ch = (char)s.Peek();
				if (ch >= '0' && ch <= '7') {
					value = (value * 8) + (ch - '0');
					s.Read();
				}
				else
					break;
			}

			return value;
		}

		static int ReadCharacterLiteralAsNumber (TextReader s)
		{
			char ch = (char)s.Peek();
			Console.WriteLine ("ch = {0}", ch);
			if (ch == '\\') {
				// escape sequence
				s.Read(); // read the slash

				ch = (char)s.Read(); // unconditionally read the next character
				Console.WriteLine ("ch = {0}", ch);

				switch (ch) {
				case 'a':
				case 'A': {
					if (s.Peek() == '-') {
						// consume the A- prefix
						ch = (char)s.Read();
						Console.WriteLine ("ch = {0}", ch);
						if (ch != '-')
							throw new Exception ("invalid escape sequence");

						// read the next part of the character
						ch = (char)ReadCharacterLiteralAsNumber(s);
						return ch | 0x400000;
					}
					else {
						// ?\a ⇒ 7                 ; control-g, C-g
						return 7;
					}
				}
				case 'b': // ?\b ⇒ 8                 ; backspace, <BS>, C-h
					return 8;
				case 't': // ?\t ⇒ 9                 ; tab, <TAB>, C-i
					return 9;
				case 'n': // ?\n ⇒ 10                ; newline, C-j
					return 10;
				case 'v': // ?\v ⇒ 11                ; vertical tab, C-k
					return 11;
				case 'f': // ?\f ⇒ 12                ; formfeed character, C-l
					return 12;
				case 'r': // ?\r ⇒ 13                ; carriage return, <RET>, C-m
					return 13;
				case 'e': // ?\e ⇒ 27                ; escape character, <ESC>, C-[
					return 27;
				case 's':
				case 'S': {
					if (s.Peek() == '-') {
						// consume the S- prefix
						ch = (char)s.Read();
						Console.WriteLine ("ch = {0}", ch);
						if (ch != '-')
							throw new Exception ("invalid escape sequence");

						// read the next part of the character
						ch = (char)ReadCharacterLiteralAsNumber(s);
						return ch | 0x2000000;
					}
					else {
						// ?\s ⇒ 32                ; space character, <SPC>
						return 32;
					}
				}
				case 'd': // ?\d ⇒ 127               ; delete character, <DEL>
					return 127;
				case 'x': {
					return ReadHexNumber (s);
				}
				case 'U': {
					int value = ReadHexNumber (s);
					if (value > 0x10ffff)
						throw new Exception ("unicode codepoint out of acceptable range");
					return value;
				}
				case '^': {
					ch = (char)s.Read();
					Console.WriteLine ("ch = {0}", ch);
					return Char.ToUpper(ch)-'A';
				}
				case 'c':
				case 'C': {
					// consume the C- prefix
					ch = (char)s.Read();
					if (ch != '-')
						throw new Exception ("invalid escape sequence");

					// read the next part of the character
					ch = (char)ReadCharacterLiteralAsNumber(s);

					// and turn it into a control character

					// if it's an ascii letter, return ToUpper(ch)-'A'
					// if it's anything else, | it with 0x40000000
					if (ch >= 'a' && ch <= 'z')
						return ch - 'a';
					else if (ch >= 'A' && ch <= 'Z')
						return ch - 'A';
					else
						return ch | 0x40000000;
				}
				case 'm':
				case 'M': {
					// consume the M- prefix
					ch = (char)s.Read();
					Console.WriteLine ("ch = {0}", ch);
					if (ch != '-')
						throw new Exception ("invalid escape sequence");

					// read the next part of the character
					ch = (char)ReadCharacterLiteralAsNumber(s);
					
					return ch | 0x8000000;
				}
				case 'h':
				case 'H': {
					// consume the H- prefix
					ch = (char)s.Read();
					Console.WriteLine ("ch = {0}", ch);
					if (ch != '-')
						throw new Exception ("invalid escape sequence");

					// read the next part of the character
					ch = (char)ReadCharacterLiteralAsNumber(s);
					return ch | 0x1000000;
				}
				case '0': case '1': case '2': case '3': case '4': case '5': case '6': case '7': {
					return ReadOctalNumber (s, ch - '0');
				}
				default:
					return ReturnCharOrThrow (s, ch);
				}
			}
			else {
				s.Read(); // read the value
				return ReturnCharOrThrow (s, ch);
			}
		}

		private static Shelisp.Number ReadCharacterLiteral (TextReader s)
		{
			return new Shelisp.Number (ReturnCharOrThrow (s, ReadCharacterLiteralAsNumber(s)));
		}

		private static Shelisp.String ReadString (TextReader s)
		{
			StringBuilder sb = new StringBuilder ();

			// consume the initial "
			s.Read();

			bool escaped = false;
			while (true) {
				if (escaped) {
					// make sure the control code is valid and append it
					sb.Append ((char)s.Read());
					escaped = false;
				}
				else {
					if (s.Peek() == '\\') {
						s.Read();
						escaped = true;
					}
					else if (s.Peek() == '"') {
						// make sure to consume the last "
						s.Read();
						break;
					}
					else
						sb.Append ((char)s.Read());
				}
			}

			return new String (sb.ToString());
		}

		private static Shelisp.Object ReadList (StreamReader s)
		{
			Debug.Print ("ReadList>");
			// consume the (
			s.Read();

			List<Shelisp.Object> objs = new List<Shelisp.Object>();
			Shelisp.Object obj;
			while ((obj = Read (s, ')')) != null) {
				Debug.Print ("+ {0}", obj);
				objs.Add (obj);
			}

			var rv = L.make_list (objs.ToArray());
			Debug.Print ("ReadList returning {0}", rv);
			return rv;
		}

		private static Shelisp.Vector ReadVector (StreamReader s)
		{
			Debug.Print ("ReadVector>");
			// consume the [
			s.Read();

			List<Shelisp.Object> objs = new List<Shelisp.Object>();
			Shelisp.Object obj;
			while ((obj = Read (s, ']')) != null) {
				Debug.Print ("+ {0}", obj);
				objs.Add (obj);
			}

			var rv = new Vector (objs.ToArray());
			Debug.Print ("ReadList returning {0}", rv);
			return rv;
		}

		[LispBuiltin ("read", MinArgs = 0)]
		public static Shelisp.Object Fread (L l, Shelisp.Object stream)
		{
#if notyet
			if (stream == null || L.NILP (stream))
				stream = standard_input.Eval(); // standard-input is a variable that by default is 't', so the minibuffer

			if (stream is Buffer)
				stream = ...; // read from the entire buffer
			else if (stream is Marker)
				stream = ...; // read the buffer starting at the marker.  the point has no effect
			else if ((stream is List && L.Qlambda.LispEq (Fcar (l, (List)stream))) ||
				 (stream is Symbol && !L.Qunbound.LispEq (((Symbol)stream).function)))
				stream = ...; // function
			else if (L.Qt.LispEq (stream))
				stream = ...; // minibuffer
#endif

			// XXX
			return L.Qnil;
		}

#if false
		[LispBuiltin ("read-from-string", MinArgs = 1)]
		public static Shelisp.Object Fread_from_string (L l, Shelisp.Object str, Shelisp.Object start, Shelisp.Object end)
		{
			if (!(str is String))
				throw new WrongTypeArgumentException ("stringp", str);
			Shelisp.Object obj;
			int pos;

			obj = Reader.Read ((string)(Shelisp.String)str, out pos);

			return new List (obj, (Number)pos);
		}
#endif
	}
}
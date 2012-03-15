using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Shelisp {
	public class IO {
		[LispBuiltin ("format-.net", MinArgs = 1)]
		public static Shelisp.Object Fprint_format(L l, Shelisp.Object format, params Shelisp.Object[] args)
		{
			switch (args.Length) {
			case 0: return new Shelisp.String (string.Format ((Shelisp.String)format));
			case 1: return new Shelisp.String (string.Format ((Shelisp.String)format, args[0]));
			case 2: return new Shelisp.String (string.Format ((Shelisp.String)format, args[0], args[1]));
			case 3: return new Shelisp.String (string.Format ((Shelisp.String)format, args[0], args[1], args[2]));
			case 4: return new Shelisp.String (string.Format ((Shelisp.String)format, args[0], args[1], args[2], args[3]));
			case 5: return new Shelisp.String (string.Format ((Shelisp.String)format, args[0], args[1], args[2], args[3], args[4]));
			case 6: return new Shelisp.String (string.Format ((Shelisp.String)format, args[0], args[1], args[2], args[3], args[4], args[5]));
			case 7: return new Shelisp.String (string.Format ((Shelisp.String)format, args[0], args[1], args[2], args[3], args[4], args[5], args[6]));
			case 8: return new Shelisp.String (string.Format ((Shelisp.String)format, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]));
			case 9: return new Shelisp.String (string.Format ((Shelisp.String)format, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]));
			case 10: return new Shelisp.String (string.Format ((Shelisp.String)format, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]));
			default:
				throw new Exception ();
			}
		}

		[LispBuiltin ("print", MinArgs = 1)]
		public static Shelisp.Object Fprint(L l, Shelisp.Object obj, Shelisp.Object stream)
		{
			Console.WriteLine (obj.ToString());
			return obj;
		}

		[LispBuiltin ("message", MinArgs = 1)]
		public static Shelisp.Object Fmessage (L l, Shelisp.Object format, params Shelisp.Object[] args)
		{
			if (format is Shelisp.String) {
				int arg_num = 0;
				StringBuilder sb = new StringBuilder ();
				string format_s = (string)(Shelisp.String)format;
				for (int i = 0; i < format_s.Length; i ++) {
					if (format_s[i] == '%') {
						char specifier = format_s[++i];
						switch (specifier) {
						case '%':
							sb.Append ('%');
							break;
						case 's':
							sb.Append (args[arg_num++].ToString());
							break;
						default:
							throw new Exception (string.Format ("format {0} unsupported", specifier));
						}
					}
					else {
						sb.Append (format_s[i]);
					}
				}
				Console.WriteLine (sb.ToString());
			}
			else {
				Console.WriteLine (format);
			}

			return L.Qnil;
		}
	}
}
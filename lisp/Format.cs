using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Shelisp {
	public class Format {

		public static Shelisp.String CFormat (Shelisp.String format, params Shelisp.Object[] args)
		{
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
					case 'd':
						sb.Append (Number.ToInt(args[arg_num++]));
						break;
					case 's':
						sb.Append (args[arg_num++].ToString("princ"));
						break;
					case 'S':
						sb.Append (args[arg_num++].ToString("prin1"));
						break;
					case 'c':
						if (!Number.IsInt (args[arg_num]))
							throw new LispErrorException ("Format specifier doesn't match argument type");
						sb.Append ((char)Number.ToInt(args[arg_num++]));
						break;
					default:
						throw new Exception (string.Format ("format {0} unsupported", specifier));
					}
				}
				else {
					sb.Append (format_s[i]);
				}
			}
			return (Shelisp.String)sb.ToString();
		}
	}
}
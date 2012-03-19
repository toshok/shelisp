using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Shelisp {
	public class IO {
		[LispBuiltin]
		public static Shelisp.Object Fformat_dotnet(L l, Shelisp.Object format, params Shelisp.Object[] args)
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

		[LispBuiltin]
		public static Shelisp.Object Fformat(L l, Shelisp.Object format, params Shelisp.Object[] args)
		{
			return Format.CFormat ((Shelisp.String)format, args);
		}

		[LispBuiltin]
		public static Shelisp.Object Fprint(L l, Shelisp.Object obj, [LispOptional] Shelisp.Object stream)
		{
			Console.WriteLine (obj.ToString("prin1"));
			return obj;
		}

		[LispBuiltin]
		public static Shelisp.Object Fprin1(L l, Shelisp.Object obj, [LispOptional] Shelisp.Object stream)
		{
			Console.Write (obj.ToString("prin1"));
			return obj;
		}

		[LispBuiltin]
		public static Shelisp.Object Fprinc(L l, Shelisp.Object obj, [LispOptional] Shelisp.Object stream)
		{
			Console.Write (obj.ToString("princ"));
			return obj;
		}

		[LispBuiltin]
		public static Shelisp.Object Fterpri(L l, [LispOptional] Shelisp.Object stream)
		{
			Console.WriteLine ();
			return L.Qt;

		}

		[LispBuiltin]
		public static Shelisp.Object Fwrite_char(L l, Shelisp.Object ch, [LispOptional] Shelisp.Object stream)
		{
			Console.Write ((char)Number.ToInt(ch));
			return ch;
		}

		[LispBuiltin]
		public static Shelisp.Object Fmessage (L l, Shelisp.Object format, params Shelisp.Object[] args)
		{
			var output = format;
			if (format is Shelisp.String) {
				output = Format.CFormat ((Shelisp.String)format, args);
			}

			Console.WriteLine (output.ToString("princ"));
			return output;
		}
	}
}
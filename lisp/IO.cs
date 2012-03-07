using System;
using System.Collections;
using System.Collections.Generic;

namespace Lisp {
	class IO : Object {
		[LispBuiltin ("print-format", MinArgs = 1)]
		public static Lisp.Object Fprint_format(L l, Lisp.Object format, params Lisp.Object[] args)
		{
			switch (args.Length) {
			case 0: Console.WriteLine ((Lisp.String)format); break;
			case 1: Console.WriteLine ((Lisp.String)format, args[0]); break;
			case 2: Console.WriteLine ((Lisp.String)format, args[0], args[1]); break;
			case 3: Console.WriteLine ((Lisp.String)format, args[0], args[1], args[2]); break;
			case 4: Console.WriteLine ((String)format, args[0], args[1], args[2], args[3]); break;
			case 5: Console.WriteLine ((String)format, args[0], args[1], args[2], args[3], args[4]); break;
			case 6: Console.WriteLine ((String)format, args[0], args[1], args[2], args[3], args[4], args[5]); break;
			case 7: Console.WriteLine ((String)format, args[0], args[1], args[2], args[3], args[4], args[5], args[6]); break;
			case 8: Console.WriteLine ((String)format, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]); break;
			case 9: Console.WriteLine ((String)format, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]); break;
			case 10: Console.WriteLine ((String)format, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]); break;
			default:
				throw new Exception ();
			}

			return L.Qnil;
		}

		[LispBuiltin ("print", MinArgs = 1)]
		public static Lisp.Object Fprint(L l, Lisp.Object obj)
		{
			Console.WriteLine (obj.ToString());
			return obj;
		}
	}
}
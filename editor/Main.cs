using System;
using System.Diagnostics;
using System.IO;
using Shelisp;

namespace Shemacs.Editor {

	public class Shemacs {
		public static void Main (string[] args)
		{
			Environment.CurrentDirectory = "/Users/toshok/src/emacs/trunk/lisp";

			L l = new L ();

			L.RegisterGlobalBuiltins (typeof (Shemacs).Assembly);

			Frame f = new Frame (l);

			// create *scratch* just to be nice...
			Buffer.Fget_buffer_create (l, new Shelisp.String ("*scratch*"));

			L.Fsetq (l, L.intern ("command-line-args"), new List (L.Qquote, new List (new List ((Shelisp.String)Process.GetCurrentProcess().ProcessName, L.string_array_to_list (args)), L.Qnil)));
			L.Fsetq (l, L.intern ("emacs-version"), (Shelisp.String)"23.2");
			
			try {
				FileIO.Fload_file (l, "loadup.el");
			}
			catch (Exception e) {
				Console.WriteLine ("failed {0}", e);
			}
		}
	}

}
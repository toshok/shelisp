using System;
using System.Diagnostics;
using System.IO;
using Shelisp;

namespace Shemacs.Editor {

	public class Process {

		public Process ()
		{
			// set up the initial environment here
			foreach (string varname in System.Environment.GetEnvironmentVariables().Keys) {
				var varvalue = System.Environment.GetEnvironmentVariable(varname);

				Vinitial_environment = new List (new Shelisp.String(varname + "=" + varvalue), Vinitial_environment);
			}

			// XXX sad sad copy - reverse twice.
			// fix this when Fcopy_sequence is added, and we start passing the L to ctors
			Vprocess_environment = List.reverse (List.reverse (Vinitial_environment));
		}

		[LispBuiltin ("process-environment")]
		public Shelisp.Object Vprocess_environment = L.Qnil;

		[LispBuiltin ("initial-environment")]
		public Shelisp.Object Vinitial_environment = L.Qnil;

		[LispBuiltin ("getenv-internal", MinArgs = 1)]
		public Shelisp.Object getenv_internal (L l, Shelisp.Object envvar, Shelisp.Object env)
		{
			return L.Qnil;
		}

		[LispBuiltin (DocString = @"*File name to load inferior shells from.
Initialized from the SHELL environment variable, or to a system-dependent
default if SHELL is not set.")]
		public Shelisp.Object Vshell_file_name = (Shelisp.String)Environment.GetEnvironmentVariable ("SHELL");

		[LispBuiltin]
		public Shelisp.Object Vexec_path = (Shelisp.String)(Environment.GetEnvironmentVariable ("EMACSPATH") ?? "/usr/local/libexec/emacs/24.0.94/x86_64-apple-darwin11.3.0");

		[LispBuiltin]
		public Shelisp.Object Fuser_full_name (L l)
		{
			return (Shelisp.String)"Chris Toshok"; // XXX
		}

		[LispBuiltin (DocString = "The host name of the machine Emacs is running on.")]
		public Shelisp.Object Vsystem_name = (Shelisp.String)"localhost"; // XXX

		[LispBuiltin (DocString = "The full name of the user logged in.")]
		public Shelisp.Object Vuser_full_name = (Shelisp.String)"Chris Toshok"; // XXX

		[LispBuiltin (DocString = "The user's name, taken from environment variables if possible.")]
		public Shelisp.Object Vuser_login_name = (Shelisp.String)"toshok"; // XXX

		[LispBuiltin (DocString = "The user's name, based upon the real uid only.")]
		public Shelisp.Object Vuser_real_login_name = (Shelisp.String)"toshok"; // XXX

		[LispBuiltin (DocString = "The release of the operating system Emacs is running on.")]
		public Shelisp.Object Voperating_system_release = (Shelisp.String)"10.7.3"; // XXX
	}
}
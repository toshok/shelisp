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
	}
}
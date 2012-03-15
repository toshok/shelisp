using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Shelisp {
	public class FileIO {

		[LispBuiltin ("expand-file-name", MinArgs = 1)]
		public static Shelisp.Object Fexpand_file_name(L l, Shelisp.Object filename, Shelisp.Object directory)
		{
			// XXX
			return filename;
		}
		
		[LispBuiltin ("load", MinArgs = 1)]
		public static Shelisp.Object Fload(L l, Shelisp.Object filename, Shelisp.Object missing_ok, Shelisp.Object nomessage, Shelisp.Object nosuffix, Shelisp.Object must_suffix)
		{
			//Console.WriteLine ("load {0} {1} {2} {3} {4}", filename, missing_ok, nomessage, nosuffix, must_suffix);
			if (!((string)(String)filename).EndsWith (".el"))
				filename = new Shelisp.String (((string)(Shelisp.String)filename) + ".el");
			return Fload_file (l, filename);
		}

		[LispBuiltin ("load-file", MinArgs = 1)]
		public static Shelisp.Object Fload_file(L l, Shelisp.Object filename)
		{
			Console.WriteLine ("load-file {0}", filename);
			StreamReader reader = File.OpenText ((Shelisp.String)filename);
			try {
				while (!reader.EndOfStream) {
					var form = Reader.Read (reader);
					if (form == null)
						break;
					form.Eval (l);
				}
				return L.Qt;
			}
			catch (Exception e) {
				Console.WriteLine (e);
				throw;
				//return L.Qnil;
			}
			finally {
				Console.WriteLine ("done with {0}", filename);
			}
		}

		public static Shelisp.Object DoAutoload (L l, Shelisp.Object fun, Shelisp.Object original_fun)
		{
			string filename = (string)(Shelisp.String)L.CAR (L.CDR (fun));
			if (L.NILP (l.Vload_path)) {
				Console.WriteLine ("load path = NIL!");
				return Fload_file (l, (Shelisp.String)(filename + ".el"));
			}
			else {
				foreach (var o in (List)l.Vload_path) {
					string full_path = Path.Combine ((string)(Shelisp.String)o, filename);
					if (Path.GetExtension (full_path) != ".el")
						full_path = full_path + ".el";
					if (File.Exists (full_path))
						return Fload_file (l, (Shelisp.String)full_path);
					Console.WriteLine ("{0} not found", full_path);
				}
			}
			throw new Exception ("file not found");
		}

		[LispBuiltin ("autoload", MinArgs = 1)]
		public static Shelisp.Object Fautoload(L l, Shelisp.Object function, Shelisp.Object filename, Shelisp.Object docstring, Shelisp.Object interactive, Shelisp.Object type)
		{
			if (L.Qt.LispEq (Symbol.Ffboundp (l, function)))
				return L.Qnil;

			var autoload_function = new List (new Shelisp.Object[] { L.Qautoload, filename, docstring == null ? (Shelisp.Object)(Shelisp.String)"" : docstring, interactive == null ? L.Qnil : interactive, type == null ? L.Qnil : interactive });
			Symbol.Ffset (l, function, autoload_function);
			l.Environment = new List (new List(function, function), l.Environment);
			return autoload_function;
		}

		// Features

		[LispBuiltin ("provide", MinArgs = 1)]
		public static Shelisp.Object Fprovide(L l, Shelisp.Object feature, params Shelisp.Object[] subfeatures)
		{
			l.AddFeature (((Symbol)feature).name);
			return L.Qnil;
		}

		[LispBuiltin ("require", MinArgs = 1)]
		public static Shelisp.Object Frequire(L l, Shelisp.Object feature, Shelisp.Object filename, Shelisp.Object noerror)
		{
			if (!l.IsFeatureLoaded (((Symbol)feature).name)) {
				var filename_s = ((Symbol)feature).name + ".el";
				Fload (l, (Shelisp.String)filename_s, L.Qnil, L.Qnil, L.Qnil, L.Qnil);
			}

			return feature;
		}

		[LispBuiltin ("featurep", MinArgs = 1)]
		public static Shelisp.Object Ffeaturep(L l, Shelisp.Object feature, Shelisp.Object subfeature)
		{
			return l.IsFeatureLoaded (((Symbol)feature).name) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("features", MinArgs = 1)]
		public static Shelisp.Object Ffeatures(L l, Shelisp.Object feature, Shelisp.Object subfeature)
		{
			throw new NotImplementedException ();
		}

		[LispBuiltin ("unload-feature", MinArgs = 1)]
		public static Shelisp.Object Funload_feature(L l, Shelisp.Object feature, Shelisp.Object subfeature)
		{
			throw new NotImplementedException ();
		}

		// variable: unload-feature-special-hooks

		// Load History
		[LispBuiltin ("symbol-file", MinArgs = 1)]
		public static Shelisp.Object Fsymbol_file(L l, Shelisp.Object sym, Shelisp.Object type)
		{
			throw new NotImplementedException ();
		}

		// variable: load-history

		[LispBuiltin ("load-file-name", DocString = "Full name of file being loaded by `load'.")]
		public Shelisp.Object Vload_file_name = L.Qnil;
	}
}
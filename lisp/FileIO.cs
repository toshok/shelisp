using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Shelisp {
	public class FileIO {

		[LispBuiltin]
		public static Shelisp.Object Fexpand_file_name(L l, Shelisp.Object filename, [LispOptional] Shelisp.Object directory)
		{
			// XXX
			return filename;
		}
		
		[LispBuiltin]
		public static Shelisp.Object Fload(L l, Shelisp.Object filename, [LispOptional] Shelisp.Object missing_ok, Shelisp.Object nomessage, Shelisp.Object nosuffix, Shelisp.Object must_suffix)
		{
			//Console.WriteLine ("load {0} {1} {2} {3} {4}", filename, missing_ok, nomessage, nosuffix, must_suffix);
			if (!((string)(String)filename).EndsWith (".el"))
				filename = new Shelisp.String (((string)(Shelisp.String)filename) + ".el");
			return Fload_file (l, filename);
		}

		[LispBuiltin]
		public static Shelisp.Object Fload_file(L l, Shelisp.Object filename)
		{
			bool failed = false;

			foreach (var o in (List)l.Vload_path) {
				string full_path = Path.Combine ((string)(Shelisp.String)o, (Shelisp.String)filename);
				if (Path.GetExtension (full_path) != ".el")
					full_path = full_path + ".el";
				if (File.Exists (full_path)) {
					StreamReader reader = File.OpenText (full_path);
					while (!reader.EndOfStream) {
						var form = Reader.Read (reader);
						if (form == null)
							break;
						try {
							form.Eval (l);
						}
						catch {
							Console.WriteLine ("problematic form was {0}", form);
							throw;
						}
					}
					return L.Qt;
				}
			}
			throw new LispFileErrorException ("Cannot open load file", (string)(Shelisp.String)filename);

#if old_impl
			try {
				StreamReader reader = File.OpenText ((Shelisp.String)filename);
				while (!reader.EndOfStream) {
					var form = Reader.Read (reader);
					if (form == null)
						break;
					try {
						form.Eval (l);
					}
					catch {
						Console.WriteLine ("problematic form was {0}", form);
						throw;
					}
				}
				return L.Qt;
			}
			catch (FileNotFoundException) {
				failed = true;
				throw new LispFileErrorException ("Cannot open load file", (string)(Shelisp.String)filename);
			}
			catch (Exception) {
				failed = true;
				throw;
			}
			finally {
				if (failed)
					Console.WriteLine ("failed to load {0}", filename);
				else
					Console.WriteLine ("done with file {0}", filename);
			}
#endif
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

		[LispBuiltin]
		public static Shelisp.Object Fautoload(L l, Shelisp.Object function, [LispOptional] Shelisp.Object filename, Shelisp.Object docstring, Shelisp.Object interactive, Shelisp.Object type)
		{
			if (L.Qt.LispEq (Symbol.Ffboundp (l, function)))
				return L.Qnil;

			var autoload_function = new List (new Shelisp.Object[] { L.Qautoload, filename, docstring == null ? (Shelisp.Object)(Shelisp.String)"" : docstring, interactive == null ? L.Qnil : interactive, type == null ? L.Qnil : interactive });
			Symbol.Ffset (l, function, autoload_function);
			l.Environment = new List (new List(function, function), l.Environment);
			return autoload_function;
		}

		// Features

		[LispBuiltin]
		public static Shelisp.Object Fprovide(L l, Shelisp.Object feature, params Shelisp.Object[] subfeatures)
		{
			l.AddFeature ((Symbol)feature);
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Frequire(L l, Shelisp.Object feature, [LispOptional] Shelisp.Object filename, Shelisp.Object noerror)
		{
			if (!l.IsFeatureLoaded ((Symbol)feature)) {
				var filename_s = ((Symbol)feature).name + ".el";
				Fload (l, (Shelisp.String)filename_s, L.Qnil, L.Qnil, L.Qnil, L.Qnil);
			}

			return feature;
		}

		[LispBuiltin]
		public static Shelisp.Object Ffeaturep(L l, Shelisp.Object feature, [LispOptional] Shelisp.Object subfeature)
		{
			return l.IsFeatureLoaded ((Symbol)feature) ? L.Qt : L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Ffeatures(L l, Shelisp.Object feature, [LispOptional] Shelisp.Object subfeature)
		{
			throw new NotImplementedException ();
		}

		[LispBuiltin]
		public static Shelisp.Object Funload_feature(L l, Shelisp.Object feature, [LispOptional] Shelisp.Object subfeature)
		{
			throw new NotImplementedException ();
		}

		// variable: unload-feature-special-hooks

		// Load History
		[LispBuiltin]
		public static Shelisp.Object Fsymbol_file(L l, Shelisp.Object sym, Shelisp.Object type)
		{
			throw new NotImplementedException ();
		}

		// variable: load-history

		[LispBuiltin (DocString = @"Alist of elements (REGEXP . HANDLER) for file names handled specially.
If a file name matches REGEXP, all I/O on that file is done by calling
HANDLER.  If a file name matches more than one handler, the handler
whose match starts last in the file name gets precedence.  The
function `find-file-name-handler' checks this list for a handler for
its argument.

HANDLER should be a function.  The first argument given to it is the
name of the I/O primitive to be handled; the remaining arguments are
the arguments that were passed to that primitive.  For example, if you
do (file-exists-p FILENAME) and FILENAME is handled by HANDLER, then
HANDLER is called like this:

  (funcall HANDLER 'file-exists-p FILENAME)

Note that HANDLER must be able to handle all I/O primitives; if it has
nothing special to do for a primitive, it should reinvoke the
primitive to handle the operation ""the usual way"".
See Info node `(elisp)Magic File Names' for more details.")]
		public Shelisp.Object Vfile_name_handler_alist = L.Qnil;
		
		[LispBuiltin (DocString = "Full name of file being loaded by `load'.")]
		public Shelisp.Object Vload_file_name = L.Qnil;


		[LispBuiltin (DocString = @"Specifies whether to use the system's trash can.
When non-nil, certain file deletion commands use the function
`move-file-to-trash' instead of deleting files outright.
This includes interactive calls to `delete-file' and
`delete-directory' and the Dired deletion commands.")]
		public static bool delete_by_moving_to_trash = false;

		[LispBuiltin (DocString = @"Non-nil says auto-save a buffer in the file it is visiting, when practical.
				     Normally auto-save files are written under other names.")]
		public static bool auto_save_visited_file_name = false;

		[LispBuiltin (DocString = @"The directory for writing temporary files.")]
		public static Shelisp.Object temporary_file_directory = L.Qnil;
	}
}
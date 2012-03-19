using System;

namespace Shelisp {
	public class LispException : Exception {
		public LispException (Shelisp.Symbol sym, Shelisp.Object data = null)
		{
			Symbol = sym;
			Data = data;
		}

		public Shelisp.Symbol Symbol { get; set; }
		public Shelisp.Object Data { get; set; }

		public override string ToString ()
		{
			if (Data == null)
				return string.Format ("({0})", Symbol);
			else
				return string.Format ("({0}: {1})", Symbol, Data);
		}

		[LispBuiltin]
		public static Shelisp.Object Fsignal (L l, Shelisp.Object sym, Shelisp.Object data)
		{
			throw new LispException ((Symbol)sym, data);
		}
	}

	class LispErrorException : LispException {
		static Shelisp.Symbol _sym = L.intern ("error");
		public LispErrorException (string msg)
			: base (_sym)
		{
			Msg = msg;
		}

		public string Msg { get; set; }
		
		public override string ToString ()
		{
			return string.Format ("(error \"{0}\")", Msg);
		}
	}

	class LispVoidFunctionException : LispException {
		static Shelisp.Symbol _sym = L.intern ("void-function");
		public LispVoidFunctionException (Shelisp.Object func)
			: base (_sym)
		{
			Func = func;
		}

		public Shelisp.Object Func { get; set; }

		public override string ToString ()
		{
			return string.Format ("(void-function {0})", Func);
		}
	}

	class LispInvalidFunctionException : LispException {
		static Shelisp.Symbol _sym = L.intern ("invalid-function");
		public LispInvalidFunctionException (Shelisp.Object func)
			: base (_sym)
		{
			Func = func;
		}

		public Shelisp.Object Func { get; set; }

		public override string ToString ()
		{
			return string.Format ("(invalid-function {0})", Func);
		}
	}

	class LispVoidVariableException : LispException {
		static Shelisp.Symbol _sym = L.intern ("void-variable");
		public LispVoidVariableException (Shelisp.Object variable)
			: base (_sym)
		{
			Variable = variable;
		}

		public Shelisp.Object Variable { get; set; }

		public override string ToString ()
		{
			return string.Format ("(void-variable {0})", Variable);
		}
	}

	class LispSettingConstantException : LispException {
		static Shelisp.Symbol _sym = L.intern ("setting-constant");
		public LispSettingConstantException (Shelisp.Object constant)
			: base (_sym)
		{
			Constant = constant;
		}

		public Shelisp.Object Constant { get; set; }

		public override string ToString ()
		{
			return string.Format ("(setting-constant {0})", Constant);
		}
	}


	class LispInvalidReadSyntaxException : LispException {
		static Shelisp.Symbol _sym = L.intern ("invalid-read-syntax");
		public LispInvalidReadSyntaxException (string syntax)
			: base (_sym)
		{
			Syntax = syntax;
		}

		public string Syntax { get; set; }

		public override string ToString ()
		{
			return string.Format ("(invalid-read-syntax {0})", Syntax);
		}
	}

	class LispFileErrorException : LispException {
		static Shelisp.Symbol _sym = L.intern ("file-error");
		public LispFileErrorException (string msg, string file)
			: base (_sym)
		{
			Msg = msg;
			File = file;
		}

		public string Msg { get; set; }
		public string File { get; set; }
		
		public override string ToString ()
		{
			return string.Format ("(file-error \"{0}\" \"{1}\")", Msg, File);
		}
	}
}
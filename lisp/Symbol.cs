using System;

namespace Shelisp {
	public class Symbol : Object {
		public Symbol (string name)
		{
			this.name = name;
			this.value = L.Qunbound;
			this.function = L.Qunbound;
		}

		public override string ToString()
		{
			return name;
		}

		public string name;

		public Shelisp.Object value;
		public Shelisp.Object function;

		public override Shelisp.Object Eval (L l, Shelisp.Object env = null)
		{
			/* Look up its binding in the lexical environment.
			   We do not pay attention to the declared_special flag here, since we
			   already did that when let-binding the variable.  */
			Debug.Print ("symbol.Eval ({0})", this);
			Shelisp.Object lex_binding = List.Fassq (l, this, env ?? l.Environment);
			Debug.Print ("lex_binding = {0}", lex_binding);
			if (L.CONSP (lex_binding)) {
				Debug.Print ("list, returning {0}", L.CDR (lex_binding));
				return L.CDR (lex_binding);
			}
			else {
				Debug.Print ("symbol, returning {0}", Symbol.Fsymbol_value (l, this));
				return Symbol.Fsymbol_value (l, this);
			}
		}

		public override int GetHashCode ()
		{
			return name.GetHashCode();
		}

		public override bool Equals (object o)
		{
			if (!(o is Symbol))
				return false;
			return ((Symbol)o).name == name;
		}

		public static implicit operator Symbol (System.String str)
		{
			return new Symbol (str);
		}

		[LispBuiltin ("boundp", MinArgs = 1)]
		public static Shelisp.Object Fboundp (L l, Shelisp.Object sym)
		{
			sym = sym.Eval (l);
			Shelisp.Object lex_binding = List.Fassq (l, sym, l.Environment);
			return L.CONSP (lex_binding) ? L.Qt : L.Qnil;
		}

		[LispBuiltin ("symbol-name", MinArgs = 1)]
		public static Shelisp.Object SymbolName (L l, Shelisp.Object o)
		{
			if (!(o is Symbol))
				throw new WrongTypeArgumentException ("symbolp", o);

			return ((Symbol)o).name;
		}

		[LispBuiltin ("symbol-value", MinArgs = 1)]
		public static Shelisp.Object Fsymbol_value (L l, Shelisp.Object o)
		{
			if (!(o is Symbol))
				throw new WrongTypeArgumentException ("symbolp", o);

			return ((Symbol)o).value;
		}

		[LispBuiltin ("symbol-function", MinArgs = 1)]
		public static Shelisp.Object Fsymbol_function (L l, Shelisp.Object o)
		{
			if (!(o is Symbol))
				throw new WrongTypeArgumentException ("symbolp", o);

			return ((Symbol)o).function;
		}

		[LispBuiltin ("fset", MinArgs = 2)]
		public static Shelisp.Object Ffset (L l, Shelisp.Object sym, Shelisp.Object defn)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);
			//if (NILP (symbol) || EQ (symbol, Qt))
			//				xsignal1 (Qsetting_constant, symbol);

			((Symbol)sym).function = defn;
			return defn;
		}

		[LispBuiltin ("put", MinArgs = 3)]
		public static Shelisp.Object Fput (L l, Shelisp.Object sym, Shelisp.Object property, Shelisp.Object value)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);

			// XXX more here

			return value;
		}

	}

}
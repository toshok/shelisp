using System;
using System.Reflection;

namespace Shelisp {
	public class Symbol : Object {
		public static Symbol Unbound {
			get {
				var unbound = L.intern ("unbound");
				unbound.value = unbound;
				unbound.function = unbound;
				return unbound;
			}
		}

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

		internal NativeValue native;
		private Shelisp.Object value;
		public Shelisp.Object Value {
			get {
				return native != null ? native.Value : this.value;
			}
			set {
				if (native != null)
					native.Value = value;
				this.value = value;
				
			}
		}

		private Shelisp.Object function;
		public Shelisp.Object Function {
			get { return function; }
			set { function = value; }
		}

		public override Shelisp.Object Eval (L l, Shelisp.Object env = null)
		{
			/* Look up its binding in the lexical environment.
			   We do not pay attention to the declared_special flag here, since we
			   already did that when let-binding the variable.  */
			Shelisp.Object lex_binding = List.Fassq (l, this, env ?? l.Environment);
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
		public static Shelisp.Object Fboundp (L l, Shelisp.Object asym)
		{
			Shelisp.Symbol sym = (Shelisp.Symbol)asym;
			Shelisp.Object lex_binding = List.Fassq (l, sym, l.Environment);
			return L.CONSP (lex_binding) ? L.Qt : (sym.native == null ? (sym.value == L.Qunbound ? L.Qnil : L.Qt) : L.Qt);
		}

		[LispBuiltin ("fboundp", MinArgs = 1)]
		public static Shelisp.Object Ffboundp (L l, Shelisp.Object asym)
		{
			Shelisp.Symbol sym = (Shelisp.Symbol)asym;
			Shelisp.Object lex_binding = List.Fassq (l, sym, l.Environment);
			return L.CONSP (lex_binding) ? L.Qt : (sym.function.LispEq(L.Qunbound) ? L.Qnil : L.Qt);
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

			return ((Symbol)o).Value;
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

			if (defn == null)
				throw new Exception ("wtf");

			((Symbol)sym).Function = defn;
			return defn;
		}

		[LispBuiltin ("fmakunbound", MinArgs = 1)]
		public static Shelisp.Object Ffset (L l, Shelisp.Object sym)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);
			//if (NILP (symbol) || EQ (symbol, Qt))
			//				xsignal1 (Qsetting_constant, symbol);

			((Symbol)sym).Function = L.Qunbound;
			return sym;
		}


		[LispBuiltin ("put", MinArgs = 3)]
		public static Shelisp.Object Fput (L l, Shelisp.Object sym, Shelisp.Object property, Shelisp.Object value)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);

			// XXX more here

			return value;
		}

		// helper classes/interface for Symbol.native
		internal interface NativeValue {
			Shelisp.Object Value { get; set; }
		}

		internal class NativeFieldInfo : NativeValue {
			public NativeFieldInfo (object o, FieldInfo field)
			{
				this.o = o;
				this.field = field;
			}

			public Shelisp.Object Value {
				get { return (Shelisp.Object)field.GetValue (o); }
				set { field.SetValue (o, value); }
			}

			private object o;
			private FieldInfo field;
		}

		internal class NativePropertyInfo : NativeValue {
			public NativePropertyInfo (object o, PropertyInfo prop)
			{
				this.o = o;
				this.prop = prop;
			}

			public Shelisp.Object Value {
				get { return (Shelisp.Object)prop.GetValue (o, new object[0]); }
				set { prop.SetValue (o, value, new object[0]); }
			}

			private object o;
			private PropertyInfo prop;
		}
	
	}

}
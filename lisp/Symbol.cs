using System;
using System.Reflection;

namespace Shelisp {
	public class Symbol : Object {
		public static readonly Symbol Unbound = new Symbol (PrimitiveSymbol.Unbound);

		enum PrimitiveSymbol {
			Unbound
		}
			
		private Symbol (PrimitiveSymbol prim)
		{
			switch (prim) {
			case PrimitiveSymbol.Unbound:
				this.name = "unbound";
				this.value = this.function = this;
				break;
			default:
				throw new NotSupportedException();
			}

		}

		public Symbol (string name)
		{
			this.name = name;
			this.value = Unbound;
			this.function = Unbound;
		}

		public override string ToString(string format_type)
		{
			switch (format_type) {
			case  "princ":
				return name.ToString();
			case "prin1":
				return name.Replace(" ", "\\ ");
			default:
				return base.ToString(format_type);
			}
		}

		public string name;
		public Symbol next; /* used in Obarray.cs */

		internal INativeValue native;
		private Shelisp.Object value;
		public Shelisp.Object Value {
			get {
				return native != null ? native.Value : this.value;
			}
			set {
				if (native != null)
					native.Value = value;
				else
					this.value = value;
			}
		}

		private Shelisp.Object function;
		public Shelisp.Object Function {
			get { return function; }
			set { function = value; }
		}

		private Shelisp.Object _plist;
		private Shelisp.Object plist {
			get { return _plist ?? (_plist = L.Qnil); }
			set { if (value == null) throw new Exception (); _plist = value; }
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

		[LispBuiltin]
		public static Shelisp.Object Fmake_symbol (L l, Shelisp.Object astr)
		{
			if (!(astr is Shelisp.String))
				throw new WrongTypeArgumentException ("stringp", astr);

			return new Symbol ((string)(Shelisp.String)astr);
		}

		[LispBuiltin]
		public static Shelisp.Object Fsymbolp (L l, Shelisp.Object asym)
		{
			return asym is Symbol ? L.Qt : L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Fboundp (L l, Shelisp.Object asym)
		{
			Shelisp.Symbol sym = (Shelisp.Symbol)asym;
			Shelisp.Object lex_binding = List.Fassq (l, sym, l.Environment);
			return L.CONSP (lex_binding) ? L.Qt : (sym.native == null ? (sym.value == L.Qunbound ? L.Qnil : L.Qt) : L.Qt);
		}

		[LispBuiltin]
		public static Shelisp.Object Ffboundp (L l, Shelisp.Object asym)
		{
			Shelisp.Symbol sym = (Shelisp.Symbol)asym;
			Shelisp.Object lex_binding = List.Fassq (l, sym, l.Environment);
			return L.CONSP (lex_binding) ? L.Qt : (sym.function.LispEq(L.Qunbound) ? L.Qnil : L.Qt);
		}

		[LispBuiltin]
		public static Shelisp.Object Fsymbol_name (L l, Shelisp.Object o)
		{
			if (!(o is Symbol))
				throw new WrongTypeArgumentException ("symbolp", o);

			return ((Symbol)o).name;
		}

		[LispBuiltin]
		public static Shelisp.Object Fsymbol_plist (L l, Shelisp.Object o)
		{
			if (!(o is Symbol))
				throw new WrongTypeArgumentException ("symbolp", o);

			return ((Symbol)o).plist;
		}

		[LispBuiltin]
		public static Shelisp.Object Fsymbol_value (L l, Shelisp.Object o)
		{
			if (!(o is Symbol))
				throw new WrongTypeArgumentException ("symbolp", o);

			// constant symbols have a value that is themselves
			if (((Symbol)o).name[0] == ':')
				return o;
			var value = ((Symbol)o).Value;
			if (value.LispEq (L.Qunbound))
				throw new LispVoidVariableException (o);
			return value;
		}

		[LispBuiltin]
		public static Shelisp.Object Fsymbol_function (L l, Shelisp.Object o)
		{
			if (!(o is Symbol))
				throw new WrongTypeArgumentException ("symbolp", o);

			var func = ((Symbol)o).Function;
			if (func.LispEq (L.Qunbound))
				throw new LispVoidFunctionException (o);
			return func;
		}

		[LispBuiltin]
		public static Shelisp.Object Ffset (L l, Shelisp.Object sym, Shelisp.Object defn)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);
			//if (NILP (symbol) || EQ (symbol, Qt))
			//				xsignal1 (Qsetting_constant, symbol);

			((Symbol)sym).Function = defn;
			return defn;
		}

		[LispBuiltin]
		public static Shelisp.Object Ffmakunbound (L l, Shelisp.Object sym)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);
			//if (NILP (symbol) || EQ (symbol, Qt))
			//				xsignal1 (Qsetting_constant, symbol);

			((Symbol)sym).Function = L.Qunbound;
			return sym;
		}


		[LispBuiltin]
		public static Shelisp.Object Fput (L l, Shelisp.Object asym, Shelisp.Object property, Shelisp.Object value)
		{
			if (!(asym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", asym);

			Symbol sym = (Symbol)asym;

			sym.plist = Plist.Fplist_put (l, sym.plist, property, value);

			return value;
		}

		[LispBuiltin]
		public static Shelisp.Object Fget (L l, Shelisp.Object sym, Shelisp.Object property)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);

			return Plist.Fplist_get (l, ((Symbol)sym).plist, property);
		}

		[LispBuiltin (DocString = @"Return t if SYMBOL has a non-void default value.
This is the value that is seen in buffers that do not have their own values
for this variable.")]
		public static Shelisp.Object Fdefault_boundp (L l, Shelisp.Object sym)
		{
			// XXX not implemented properly
			return ((Symbol)sym).Value.LispEq (L.Qunbound) ? L.Qnil : L.Qt;
		}

		[LispBuiltin (DocString = @"Return SYMBOL's default value.
This is the value that is seen in buffers that do not have their own values
for this variable.  The default value is meaningful for variables with
local bindings in certain buffers.")]
		public static Shelisp.Object Fdefault_value (L l, Shelisp.Object sym)
		{
			// XXX not implemented properly
			var val = ((Symbol)sym).Value;
			if (val.LispEq (L.Qunbound))
				throw new LispVoidVariableException (sym);
			return ((Symbol)sym).Value;
		}

		[LispBuiltin (DocString = @"Return t if OBJECT is a keyword.
This means that it is a symbol with a print name beginning with `:'
interned in the initial obarray.")]
		public static Shelisp.Object Fkeywordp (L l, Shelisp.Object sym)
		{
			// XXX initial obarray?
			return ((sym is Symbol) && ((Symbol)sym).name[0] == ':') ? L.Qt : L.Qnil;
		}



		// helper classes/interface for Symbol.native
		internal interface INativeValue {
			Shelisp.Object Value { get; set; }
		}

		internal class NativeFieldInfoBase {
			public NativeFieldInfoBase (object o, FieldInfo field)
			{
				this.o = o;
				this.field = field;
			}

			protected object o;
			protected FieldInfo field;
		}

		internal class NativeFieldInfo : NativeFieldInfoBase, INativeValue {
			public NativeFieldInfo (object o, FieldInfo field) : base (o, field) { }
			public Shelisp.Object Value {
				get { return (Shelisp.Object)field.GetValue (o); }
				set { field.SetValue (o, value); }
			}
		}
		internal class NativeBoolFieldInfo : NativeFieldInfoBase, INativeValue {
			public NativeBoolFieldInfo (object o, FieldInfo field) : base (o, field) { }
			public Shelisp.Object Value {
				get { return (bool)field.GetValue (o) ? L.Qt : L.Qnil; }
				set { field.SetValue (o, L.NILP(value) ? false : true); }
			}
		}
		internal class NativeIntFieldInfo : NativeFieldInfoBase, INativeValue {
			public NativeIntFieldInfo (object o, FieldInfo field) : base (o, field) { }
			public Shelisp.Object Value {
				get { return new Number((int)field.GetValue (o)); }
				set { field.SetValue (o, (int)((Number)value).boxed); }
			}
		}
		internal class NativeFloatFieldInfo : NativeFieldInfoBase, INativeValue {
			public NativeFloatFieldInfo (object o, FieldInfo field) : base (o, field) { }
			public Shelisp.Object Value {
				get { return new Number((float)field.GetValue (o)); }
				set { field.SetValue (o, (float)((Number)value).boxed); }
			}
		}



		internal class NativePropertyInfoBase {
			public NativePropertyInfoBase (object o, PropertyInfo property)
			{
				this.o = o;
				this.property = property;
			}

			protected object o;
			protected PropertyInfo property;
		}

		internal class NativePropertyInfo : NativePropertyInfoBase, INativeValue {
			public NativePropertyInfo (object o, PropertyInfo property) : base (o, property) { }
			public Shelisp.Object Value {
				get { return (Shelisp.Object)property.GetValue (o, new object[0]); }
				set { property.SetValue (o, value, new object[0]); }
			}
		}
		internal class NativeBoolPropertyInfo : NativePropertyInfoBase, INativeValue {
			public NativeBoolPropertyInfo (object o, PropertyInfo property) : base (o, property) { }
			public Shelisp.Object Value {
				get { return (bool)property.GetValue (o, new object[0]) ? L.Qt : L.Qnil; }
				set { property.SetValue (o, L.NILP(value) ? false : true, new object[0]); }
			}
		}
		internal class NativeIntPropertyInfo : NativePropertyInfoBase, INativeValue {
			public NativeIntPropertyInfo (object o, PropertyInfo property) : base (o, property) { }
			public Shelisp.Object Value {
				get { return new Number((int)property.GetValue (o, new object[0])); }
				set { property.SetValue (o, (int)((Number)value).boxed, new object[0]); }
			}
		}
		internal class NativeFloatPropertyInfo : NativePropertyInfoBase, INativeValue {
			public NativeFloatPropertyInfo (object o, PropertyInfo property) : base (o, property) { }
			public Shelisp.Object Value {
				get { return new Number((float)property.GetValue (o, new object[0])); }
				set { property.SetValue (o, (float)((Number)value).boxed, new object[0]); }
			}
		}
	}

}
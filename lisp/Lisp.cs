using System;
using SysArray = System.Array;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Shelisp {

	public class L {
		static Vector obarray;
		static Vector current_obarray;

		static L ()
		{
			obarray = new Vector (65, new Number (0));
			current_obarray = obarray;

			// must come first
			Qunbound = Symbol.Unbound;

			Qcdr = DEFSYM ("cdr");
			Qcar = DEFSYM ("car");
			Qlambda = DEFSYM ("lambda");
			Qclosure = DEFSYM ("closure");
			Qmacro = DEFSYM ("macro");
			Qquote = DEFSYM ("quote");
			Qautoload = DEFSYM ("autoload");
			Qtest = DEFSYM (":test");
			Qweakness = DEFSYM (":weakness");
			Qsize = DEFSYM (":size");
			Qrehash_size = DEFSYM (":rehash-size");
			Qrehash_threshold = DEFSYM (":rehash-threshold");

			Qkey = DEFSYM ("key");
			Qvalue = DEFSYM ("value");
			Qkey_or_value = DEFSYM ("key-or-value");
			Qkey_and_value = DEFSYM ("key-and-value");

			Qt = DEFSYM ("t");
			((Symbol)Qt).Value = Qt;
			Qnil = DEFSYM ("nil");
			((Symbol)Qnil).Value = Qnil;

			variable_container_types = new List<Type>();

			root_environment = Qnil;
			RegisterGlobalBuiltins (typeof (L).Assembly);

#if DEBUG
			// dump out our environment
			if (root_environment != Qnil) {
				foreach (var binding in (List)root_environment) {
					if (NILP(binding))
						break;

					Debug.Print ("{0}", CAR(binding));
				}
			}
#endif

		}

		public L ()
		{
			Environment = root_environment;

			variable_containers = new List<object>();

			foreach (var t in variable_container_types) {
				object o;
				if (t == GetType())
					o = this;
				else
					o = Activator.CreateInstance (t);
				RegisterVariables (t, o);
				variable_containers.Add (o);
			}
		}

		public static void RegisterGlobalBuiltins (Assembly assembly)
		{
			foreach (var t in assembly.GetTypes())
				RegisterGlobalBuiltins (t);
		}

		static bool IsLispOptional (ParameterInfo p)
		{
			return Attribute.GetCustomAttribute (p, typeof (LispOptionalAttribute), true) != null;
		}

		static bool IsLispRest (ParameterInfo p)
		{
			return Attribute.GetCustomAttribute (p, typeof (LispRestAttribute), true) != null;
		}

		static bool IsParams (ParameterInfo p)
		{
			return Attribute.GetCustomAttribute (p, typeof (ParamArrayAttribute), true) != null;
		}


		public static void RegisterGlobalBuiltins (Type t)
		{
			foreach (var method in t.GetMethods()) {
				var builtin_attrs = method.GetCustomAttributes (typeof (LispBuiltinAttribute), true);
				if (builtin_attrs == null || builtin_attrs.Length == 0)
					continue;
				foreach (var ba in builtin_attrs) {
					LispBuiltinAttribute builtin_attr = (LispBuiltinAttribute)ba;

					var lisp_name = builtin_attr.Name ?? method.Name.Replace("_", "-");
					if (lisp_name.StartsWith ("F"))
						lisp_name = lisp_name.Substring (1);

					Debug.Print ("found [LispBuiltin({0})] on method '{1}.{2}'", lisp_name, method.DeclaringType, method.Name);

					string doc_string = builtin_attr.DocString ?? "";
					int min_args = builtin_attr.MinArgs;
					if (min_args == -1) {
						// compute the minimum args
						var parameters = method.GetParameters ();
						for (int i = 1/*we skip the `L l' first parameter*/; i < parameters.Length; i ++) {
							if (IsLispOptional (parameters[i])) {
								min_args = i-1; // -1 to remove the `L l' parameter
								break;
							}
							else if (IsParams (parameters[i])) {
								min_args = i-1; // -1 to remove the `L l' parameter
								break;
							}
						}
					}

					Symbol s = L.intern (lisp_name);
					s.Function = L.DEFUN_internal (lisp_name, doc_string, min_args, builtin_attr.Unevalled, method);

					root_environment = new List (new List (s, s), root_environment);
				}
			}

			bool has_instance_variables = false;
			bool has_static_variables = false;

			// check if the type defines any builtin properties or fields.  if so, create an instance of it, register the builtins,
			// and add the instance to the variable_container_types list.
			foreach (var field in t.GetFields()) {
				var builtin_attrs = field.GetCustomAttributes (typeof (LispBuiltinAttribute), true);
				if (builtin_attrs != null && builtin_attrs.Length != 0) {
					if (field.IsStatic)
						has_static_variables = true;
					else
						has_instance_variables = true;

					if (has_static_variables && has_instance_variables)
						break;
				}
			}
			if (!has_static_variables || !has_instance_variables) {
				foreach (var property in t.GetProperties()) {
					var builtin_attrs = property.GetCustomAttributes (typeof (LispBuiltinAttribute), true);
					if (builtin_attrs != null && builtin_attrs.Length != 0) {
						if (property.GetGetMethod().IsStatic)
							has_static_variables = true;
						else
							has_instance_variables = true;

						if (has_static_variables && has_instance_variables)
							break;
					}
				}
			}

			if (has_instance_variables)
				variable_container_types.Add (t);
			if (has_static_variables)
				RegisterVariables (t, null);
		}

		public static void RegisterVariables (Type type, object o)
		{
			foreach (var field in type.GetFields()) {
				if (o == null && !field.IsStatic)
					continue;
				var builtin_attrs = field.GetCustomAttributes (typeof (LispBuiltinAttribute), true);
				if (builtin_attrs == null || builtin_attrs.Length == 0)
					continue;
				foreach (var ba in builtin_attrs) {
					LispBuiltinAttribute builtin_attr = (LispBuiltinAttribute)ba;

					var lisp_name = builtin_attr.Name ?? field.Name.Replace("_", "-");
					if (lisp_name.StartsWith ("V"))
						lisp_name = lisp_name.Substring (1);

					Console.WriteLine ("found [LispBuiltin({0})] on field '{1}.{2}'", lisp_name, field.DeclaringType, field.Name);

					string doc_string = builtin_attr.DocString ?? "";

					Symbol s = L.intern (lisp_name);
					if (field.FieldType == typeof(bool))
						s.native = new Symbol.NativeBoolFieldInfo (o, field);
					else if (field.FieldType == typeof(int))
						s.native = new Symbol.NativeIntFieldInfo (o, field);
					else if (field.FieldType == typeof(float))
						s.native = new Symbol.NativeFloatFieldInfo (o, field);
					else
						s.native = new Symbol.NativeFieldInfo (o, field);
				}
			}

			foreach (var property in type.GetProperties()) {
				if (o == null && !property.GetGetMethod().IsStatic)
					continue;
				var builtin_attrs = property.GetCustomAttributes (typeof (LispBuiltinAttribute), true);
				if (builtin_attrs == null || builtin_attrs.Length == 0)
					continue;
				foreach (var ba in builtin_attrs) {
					LispBuiltinAttribute builtin_attr = (LispBuiltinAttribute)ba;

					var lisp_name = builtin_attr.Name ?? property.Name.Replace("_", "-");
					if (lisp_name.StartsWith ("V"))
						lisp_name = lisp_name.Substring (1);

					Debug.Print ("found [LispBuiltin({0})] on property '{1}.{2}'", lisp_name, property.DeclaringType, property.Name);

					string doc_string = builtin_attr.DocString ?? "";

					Symbol s = L.intern (lisp_name);
					if (property.PropertyType == typeof(bool))
						s.native = new Symbol.NativeBoolPropertyInfo (o, property);
					else if (property.PropertyType == typeof(int))
						s.native = new Symbol.NativeIntPropertyInfo (o, property);
					else if (property.PropertyType == typeof(float))
						s.native = new Symbol.NativeFloatPropertyInfo (o, property);
					else
						s.native = new Symbol.NativePropertyInfo (o, property);
				}
			}
		}

		[LispBuiltin]
		public Shelisp.Object Vfeatures = L.make_list (L.intern ("emacs"));


		public bool IsFeatureLoaded (Symbol feature)
		{
			foreach (var feature_sym in (List)Vfeatures) {
				if (((Symbol)feature_sym).LispEq(feature))
					return true;
			}

			return false;
		}

		public void AddFeature (Symbol feature)
		{
			if (!IsFeatureLoaded (feature)) {
				Console.WriteLine ("AddFeature {0}", feature);
				Vfeatures = new List (feature, Vfeatures);
			}
		}

		// the types we've made note of as having variables.  these are
		// instantiated and registered when a new L instance is created.
		private static List<Type> variable_container_types;

		// this is the list containing the instantiated container types.
		// we don't care about them, we just need to protect them from being
		// GC'ed for the lifetime of this L.
		private List<object> variable_containers;

		// our current executing environment
		public Shelisp.Object Environment { get; set; }

		// the environment that contains all the builtin definitions
		static Shelisp.Object root_environment;

		public static Shelisp.Object Qcdr;
		public static Shelisp.Object Qcar;
		public static Shelisp.Object Qt;
		public static Shelisp.Object Qnil;
		public static Shelisp.Object Qlambda;
		public static Shelisp.Object Qmacro;
		public static Shelisp.Object Qunbound;
		public static Shelisp.Object Qquote;
		public static Shelisp.Object Qclosure;
		public static Shelisp.Object Qautoload;

		public static Shelisp.Symbol Qtest;
		public static Shelisp.Symbol Qweakness;
		public static Shelisp.Symbol Qsize;
		public static Shelisp.Symbol Qrehash_size;
		public static Shelisp.Symbol Qrehash_threshold;

		public static Shelisp.Symbol Qkey;
		public static Shelisp.Symbol Qvalue;
		public static Shelisp.Symbol Qkey_or_value;
		public static Shelisp.Symbol Qkey_and_value;

		// [i*2] = beginning of match
		// [i*2+1] = end of match
		public static int[] match_data;

		[LispBuiltin ("obarray")]
		public static Shelisp.Object CurrentObarray {
			get { return current_obarray; }
		}

		public static Shelisp.Symbol intern (string str)
		{
			return intern (str, obarray);
		}

		public static Shelisp.Symbol intern (string str, Vector obarray)
		{
			return Obarray.Intern (obarray, str);
		}

		public static Shelisp.Object intern_soft (string str)
		{
			return intern_soft (str, obarray);
		}

		public static Shelisp.Object intern_soft (string str, Vector obarray)
		{
			return Obarray.InternSoft (obarray, str);
		}

		public static Shelisp.Object string_array_to_list (string[] arr)
		{
			Object cons = Qnil;
			for (int i = arr.Length - 1; i >= 0; i --)
				cons = new List (new Shelisp.String (arr[i]), cons);
			return cons;
		}

		public static Shelisp.Object int_array_to_list (int[] arr)
		{
			Object cons = Qnil;
			for (int i = arr.Length - 1; i >= 0; i --)
				cons = new List (new Shelisp.Number (arr[i]), cons);
			return cons;
		}

		public static Shelisp.Object make_list_atom_tail (params Shelisp.Object[] arr)
		{
			if (arr.Length > 1) {
				Object cons = arr[arr.Length-1];
				for (int i = arr.Length - 2; i >= 0; i --)
					cons = new List (arr[i], cons);
				return cons;
			}
			else
				throw new Exception ("failed to make list with non-cons tail");
		}

		public static Shelisp.Object make_list (params Shelisp.Object[] arr)
		{
			Object cons = Qnil;
			for (int i = arr.Length - 1; i >= 0; i --)
				cons = new List (arr[i], cons);
			return cons;
		}

		private static Shelisp.Symbol DEFSYM (string lisp_name)
		{
			Symbol s = intern (lisp_name);
			return s;
		}

		private static Shelisp.Subr DEFUN_internal (string lisp_name, string doc, int min_args, bool unevalled, MethodInfo meth, object target = null)
		{
			Subr s = new Subr (lisp_name, doc, min_args, unevalled, meth);
			return s;
		}

		private static Shelisp.Subr DEFUN_internal (string lisp_name, string doc, int min_args, Delegate d)
		{
			return DEFUN_internal (lisp_name, doc, min_args, false, d.Method, d.Target);
		}

		public static Shelisp.Subr DEFUN (string lisp_name, string doc, Action func)
		{
			return DEFUN_internal (lisp_name, doc, 0, func);
		}

		public static Shelisp.Subr DEFUN<T> (string lisp_name, string doc, int min_args, Func<T> func)
		{
			return DEFUN_internal (lisp_name, doc, min_args, func);
		}

		public static Shelisp.Subr DEFUN<T1,T2> (string lisp_name, string doc, int min_args, Func<T1,T2> func)
		{
			return DEFUN_internal (lisp_name, doc, min_args, func);
		}

		public static Shelisp.Subr DEFUN<T1,T2,T3> (string lisp_name, string doc, int min_args, Func<T1,T2,T3> func)
		{
			return DEFUN_internal (lisp_name, doc, min_args, func);
		}

		public static Shelisp.Subr DEFUN<T1,T2,T3,T4> (string lisp_name, string doc, int min_args, Func<T1,T2,T3,T4> func)
		{
			return DEFUN_internal (lisp_name, doc, min_args, func);
		}

		public static Shelisp.Object CAR (Shelisp.Object cons)
		{
			if (!(cons is List))
				Console.WriteLine ("CAR {0}", cons);
			return ((List)cons).car;
		}

		public static Shelisp.Object CDR (Shelisp.Object cons)
		{
			return NILP(cons) ? Qnil : ((List)cons).cdr;
		}

		public static Shelisp.List CONS (Shelisp.Object cons)
		{
			return ((List)cons);
		}

		public static bool NILP (Shelisp.Object o)
		{
			return o == Qnil;
		}

		public static bool LISTP (Shelisp.Object o)
		{
			return o is List || o == Qnil;
		}

		public static bool CONSP (Shelisp.Object o)
		{
			if (o is Quote)
				return L.CONSP(((Quote)o).obj);
			return o is List;
		}

		// string foo
		public static int SCHARS (Shelisp.Object o)
		{
			return (o as String).native_string.Length;
		}

		public static char SREF (Shelisp.Object o, int c)
		{
			return (o as String).native_string[c];
		}

		static int eval_depth = 0;

		[Conditional ("EVAL_SPEW")]
		public static void EvalIndent (int specified_amount = -1)
		{
			if (specified_amount == -1)
				specified_amount = 2;
			eval_depth += specified_amount;
		}

		[Conditional ("EVAL_SPEW")]
		public static void EvalOutdent (int specified_amount = -1)
		{
			if (specified_amount == -1)
				specified_amount = 2;
			eval_depth -= specified_amount;
		}

		[Conditional ("EVAL_SPEW")]
		public static void EvalSpew (string format, params object[] args)
		{
			for (int i = 0; i < eval_depth; i ++)
				Console.Write (" ");
			Console.WriteLine (format, args);
		}

		[LispBuiltin]
		public static Shelisp.Object Ffuncall (L l, Shelisp.Object fun, params Shelisp.Object[] args)
		{
			if (fun is Symbol)
				fun = ((Symbol)fun).Function;

			if (fun is Subr) {
				L.EvalSpew ("funcall subr application, {0}", fun);

				L.EvalIndent();
				var rv =  ((Subr)fun).Call (l, args);
				L.EvalOutdent();

				L.EvalSpew ("funcall subr {0} evaluated to {1}", fun, rv);
				return rv;
			}
			else {
				L.EvalSpew ("evaluating funcall, {0}", fun);

				L.EvalIndent();
				var rv = List.ApplyLambda (l, fun, L.make_list (args), null, false/*the args have already been evaluated*/);
				L.EvalOutdent();

				L.EvalSpew ("evaluating of {0} resulted in {1}", fun, rv);
				return rv;
			}
		}

		[LispBuiltin]
		public static Shelisp.Object Fapply (L l, Shelisp.Object fun, params Shelisp.Object[] args)
		{
			//XXX typecheck everything

			// fix up the args..
			Shelisp.Object rest_args = args[args.Length - 1];
			int rest_count = (L.NILP (rest_args)) ? 0 : ((List)rest_args).Length;

			Shelisp.Object[] real_args = new Shelisp.Object[args.Length - 1 + rest_count];
			System.Array.Copy (args, 0, real_args, 0, args.Length - 1);
			if (rest_count > 0) {
				int i = args.Length - 1;
				foreach (var arg in (List)rest_args) {
					real_args[i++] = arg;
				}
			}

			if (fun is Symbol)
				fun = L.Findirect_function (l, fun);

			fun = fun.Eval(l);
			if (fun is Subr)
				return ((Subr)fun).Call (l, real_args);
			else
				return List.ApplyLambda (l, fun, L.make_list (real_args), null, false/*the args have already been evaluated*/);
		}

		[LispBuiltin]
		public static Shelisp.Object Feval (L l, Shelisp.Object obj)
		{
			return obj.Eval(l);
		}

		[LispBuiltin (Unevalled = true)]
		public static Shelisp.Object Ffunction (L l, Shelisp.Object args)
		{
			Shelisp.Object quoted = args;

#if not_ported
			if (!L.NILP (L.CDR (args)))
				xsignal2 (Qwrong_number_of_arguments, Qfunction, Flength (args));
#endif

			if (/*!L.NILP (l.Environment)
			    && */L.CONSP (quoted)
			    && L.CAR (quoted).LispEq (L.Qlambda)) {
				/* This is a lambda expression within a lexical environment;
				   return an interpreted closure instead of a simple lambda.  */
				return new List (L.Qclosure, new List (l.Environment,
								       L.CDR (quoted)));
			}
			else {
				/* Simply quote the argument.  */
				return quoted;
			}
		}

		[LispBuiltin]
		public static Shelisp.Object Finteractive (L l, params Shelisp.Object[] args)
		{
			Console.Write ("(interactive");
			foreach (var a in args)
				Console.Write (" {0}", a);
			Console.WriteLine (")");
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Ferror (L l, Shelisp.Object format, params Shelisp.Object[] args)
		{
			// we need a C-styled sprintf/string.Format here
			throw new Exception ((string)(Shelisp.String)format);
		}

		[LispBuiltin]
		public static Shelisp.Object Fadd_hook (L l, Shelisp.Object hook, Shelisp.Object function, [LispOptional] Shelisp.Object append, Shelisp.Object local)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Frun_hooks (L l, [LispRest] params Shelisp.Object[] hookvars)
		{
			// XXX
			return L.Qnil;
		}

		[LispBuiltin]
		public static Shelisp.Object Findirect_function (L l, Shelisp.Object symorfunction)
		{
			Shelisp.Object sym = symorfunction;

			Debug.Print ("indirect_function >>>>");
			while (true) {
				Debug.Print ("indirect-function for {0}", sym);
				if (sym is Symbol) {
					if (sym.LispEq (L.Qunbound)) {
						Debug.Print ("<<<<");
						return sym;
					}
					sym = ((Symbol)sym).Function;
					continue;
				}
				else {
					Debug.Print ("<<<<");
					return sym;
				}
			}
		}

		[LispBuiltin (MinArgs = 3, Unevalled = true, DocString = 
@"Define NAME as a function.
The definition is (lambda ARGLIST [DOCSTRING] BODY...).
See also the function `interactive'.
usage: (defun NAME ARGLIST [DOCSTRING] BODY...)")]
		public static Shelisp.Object Fdefun (L l, Shelisp.Object sym, Shelisp.Object arglist, params Shelisp.Object[] body_forms)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);

			var doc_string = "";

			if (body_forms.Length > 0) {
				if (body_forms[0] is Shelisp.String) {
					doc_string = (Shelisp.String)body_forms[0];
					Shelisp.Object[] new_body_forms = new Shelisp.Object[body_forms.Length - 1];
					SysArray.Copy (body_forms, 1, new_body_forms, 0, new_body_forms.Length);
					body_forms = new_body_forms;
				}
			}
			if (body_forms.Length == 0)
				throw new Exception ("0 length body forms");

			Shelisp.Object defn = List.Fnconc (l, L.make_list (L.Qlambda, arglist), new List (body_forms));

			Symbol s = (Symbol)sym;
			Symbol.Ffset (l, s, defn);
			// XXX more here I'm sure... like what do we do with the doc string?
			//l.Environment = new List (new List(s, s), l.Environment);

			return sym;
		}

		[LispBuiltin]
		public static Shelisp.Object Fdefalias (L l, Shelisp.Object name, Shelisp.Object defn, [LispOptional] Shelisp.Object doc_string)
		{
			if (!(name is Symbol))
				throw new WrongTypeArgumentException ("symbolp", name);

			Shelisp.Object fun;

			if (defn is Symbol)
				fun = ((Symbol)defn).Function;
			else
				fun = defn;

			Symbol s = (Symbol)name;
			Symbol.Ffset (l, s, defn);

			// XXX more here I'm sure... like what do we do with the doc string?
			//l.Environment = new List (new List(s, s), l.Environment);

			return fun;
		}

		[LispBuiltin (Unevalled = true)]
		public static Shelisp.Object Fdefconst (L l, Shelisp.Object sym, Shelisp.Object value, [LispOptional] Shelisp.Object doc_string)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);
			((Symbol)sym).Value = value.Eval (l);
			return sym;
		}

		[LispBuiltin (MinArgs = 2, Unevalled = true)]
		public static Shelisp.Object Fdefmacro (L l, Shelisp.Object sym, Shelisp.Object arglist, params Shelisp.Object[] body_forms)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);

			var doc_string = "";

			if (body_forms.Length > 0) {
				if (body_forms[0] is Shelisp.String) {
					doc_string = (Shelisp.String)body_forms[0];
					Shelisp.Object[] new_body_forms = new Shelisp.Object[body_forms.Length - 1];
					SysArray.Copy (body_forms, 1, new_body_forms, 0, new_body_forms.Length);
					body_forms = new_body_forms;
				}
			}
			Shelisp.Object defn = List.Fnconc (l, L.make_list (L.Qmacro, L.Qlambda, arglist), body_forms.Length == 0 ? L.Qnil : new List (body_forms));

			Symbol s = (Symbol)sym;
			Symbol.Ffset (l, s, defn);
			// XXX more here I'm sure... like what do we do with the doc string?
			//l.Environment = new List (new List(s, s), l.Environment);

			return sym;
		}

		[LispBuiltin (Unevalled = true)]
		public static Shelisp.Object Fdefvar (L l, Shelisp.Object sym, [LispOptional] Shelisp.Object value, Shelisp.Object docstring)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);
			((Symbol)sym).Value = L.NILP(value) ? L.Qnil : value.Eval (l);
			return sym;
		}

		[LispBuiltin]
		public static Shelisp.Object Fdefvaralias (L l, Shelisp.Object name, Shelisp.Object base_variable, [LispOptional] Shelisp.Object doc_string)
		{
			if (!(name is Symbol))
				throw new WrongTypeArgumentException ("symbolp", name);

			Shelisp.Object value;

			if (base_variable is Symbol)
				value = ((Symbol)base_variable).Value;
			else
				value = base_variable;

			Symbol s = (Symbol)name;
			L.Fsetq (l, s, value);

			// XXX more here I'm sure... like what do we do with the doc string?
			//l.Environment = new List (new List(s, s), l.Environment);

			return base_variable;
		}

		[LispBuiltin (Unevalled = true)]
		public static Shelisp.Object Fsetq_default (L l, params Shelisp.Object[] sym_vals)
		{
			Shelisp.Object evalled = L.Qnil;
			for (int i = 0; i < sym_vals.Length; i += 2) {
				var sym = sym_vals[i];

				if (!(sym is Symbol))
					throw new WrongTypeArgumentException ("symbolp", sym);

				var val = i < sym_vals.Length - 1 ? sym_vals[i+1] : L.Qnil;

				evalled = val.Eval(l);
				((Symbol)sym).Value = evalled;
			}

			return evalled;
		}

		[LispBuiltin (Unevalled = true)]
		public static Shelisp.Object Fset_default (L l, params Shelisp.Object[] sym_vals)
		{
			Shelisp.Object val = L.Qnil;
			for (int i = 0; i < sym_vals.Length; i += 2) {
				var sym = sym_vals[i];

				if (!(sym is Symbol)) {
					sym = sym.Eval(l);
					if (!(sym is Symbol))
						throw new WrongTypeArgumentException ("symbolp", sym);
				}

				val = i < sym_vals.Length - 1 ? sym_vals[i+1] : L.Qnil;

				((Symbol)sym).Value = val;
			}

			return val;
		}

		[LispBuiltin (Unevalled = true)]
		public static Shelisp.Object Fsetq (L l, params Shelisp.Object[] sym_vals)
		{
			Shelisp.Object evalled = L.Qnil;
			for (int i = 0; i < sym_vals.Length; i += 2) {
				var sym = sym_vals[i];

				if (!(sym is Symbol))
					throw new WrongTypeArgumentException ("symbolp", sym);

				var val = i < sym_vals.Length - 1 ? sym_vals[i+1] : L.Qnil;

				evalled = val.Eval(l);
				// if the symbol exists in our environment it there.
				// otherwise set the global (on the symbol).
				Shelisp.Object lex_binding = List.Fassq (l, sym, l.Environment);
				if (L.CONSP (lex_binding)) {
					((List)lex_binding).cdr = evalled;
				}
				else {
					((Shelisp.Symbol)sym).Value = evalled;
				}
			}

			return evalled;
		}

		[LispBuiltin]
		public static Shelisp.Object Fset (L l, Shelisp.Object sym, Shelisp.Object val)
		{
			if (!(sym is Symbol))
				throw new WrongTypeArgumentException ("symbolp", sym);

			l.Environment = new List (new List(sym, val), l.Environment);
			return val;
		}

		[LispBuiltin (Unevalled = true)]
		public static Shelisp.Object Flet (L l, Shelisp.Object bindings, params Shelisp.Object[] forms)
		{
			Shelisp.Object prev_environment = l.Environment;
			Shelisp.Object new_environment = l.Environment;

			if (L.CONSP (bindings)) {
				foreach (var binding in (List)bindings) {
					Shelisp.Object sym, value;
					if (L.CONSP (binding)) {
						sym = L.CAR(binding);
						Debug.Print ("sym = {0}, binding = {1}", sym, L.CAR(L.CDR (binding)));
						value = L.CAR(L.CDR (binding)).Eval(l, prev_environment);
					}
					else {
						sym = binding;
						value = L.Qnil;
					}
					Debug.Print ("adding binding from {0} to {1}", sym, value);
					new_environment = new List (new List (sym, value), new_environment);
				}
			}

			l.Environment = new_environment;

			Debug.Print ("evaluating body forms");
			Shelisp.Object rv = L.Qnil;
			foreach (var o in forms) {
				Debug.Print ("form = {0}", o);
				rv = o.Eval (l);
			}

			l.Environment = prev_environment;

			return rv;
		}

		[LispBuiltin ("let*", Unevalled = true)]
		public static Shelisp.Object Flet_star (L l, Shelisp.Object bindings, params Shelisp.Object[] forms)
		{
			Shelisp.Object prev_environment = l.Environment;

			if (L.CONSP (bindings)) {
				foreach (var binding in (List)bindings) {
					Shelisp.Object sym, value;
					if (L.CONSP (binding)) {
						sym = L.CAR(binding);
						value = L.CAR(L.CDR (binding)).Eval(l, l.Environment);
					}
					else {
						sym = binding;
						value = L.Qnil;
					}
					l.Environment = new List (new List (sym, value), l.Environment);
				}
			}

			Shelisp.Object rv = L.Qnil;
			foreach (var o in forms)
				rv = o.Eval (l);

			l.Environment = prev_environment;

			return rv;
		}

		[LispBuiltin]
		public static Shelisp.Object Fcurrent_time (L l)
		{
			DateTime unixRef = new DateTime(1970, 1, 1, 0, 0, 0);

			long ticks_unix = (DateTime.Now.Ticks - unixRef.Ticks);

			int high, low, microseconds;
			int seconds;

			seconds = (int)(ticks_unix / TimeSpan.TicksPerSecond);
			microseconds = (int)(ticks_unix % TimeSpan.TicksPerSecond);

			high = (int)(seconds / (2<<16));
			low = (int)(seconds % (2<<16));

			return L.make_list (high, low, microseconds);
		}

		[LispBuiltin]
		public static Shelisp.Object Fsystem_name (L l)
		{
			// XXX
			return (Shelisp.String)"localhost";
		}

		[LispBuiltin]
		public static Shelisp.Object Fgarbage_collect (L l)
		{
			GC.Collect();
			Console.WriteLine ("garbage-collect statistics not here yet");
			return L.Qnil;
		}

		[LispBuiltin]
		public Shelisp.Object Vgc_cons_threshold = new Number (Int32.MaxValue);

		[LispBuiltin]
		public bool Vgarbage_collection_messages = false;

		static Shelisp.Object find_matching_handler (LispException e, Shelisp.Object[] handlers)
		{
			foreach (var handler in handlers) {
				var error_match_list = L.CAR (handler);

				if (L.NILP (error_match_list))
					continue;

				if (L.CONSP (error_match_list)) {
					if (!L.NILP (List.Memq (e.Symbol, (Shelisp.List)error_match_list))) {
						return L.CDR (handler);
					}
				}
				else if (error_match_list.LispEq (e.Symbol)) {
					return L.CDR (handler);
				}
			}

			return L.Qnil;
		}

		[LispBuiltin (Unevalled = true)]
		public static Shelisp.Object Fcondition_case (L l, Shelisp.Object var, Shelisp.Object protected_form, params Shelisp.Object[] handlers)
		{
			try {
				return protected_form.Eval(l);
			}
			catch (LispException e) {
				/* find a handler that handles the error symbol */
				Shelisp.Object handler_forms = find_matching_handler (e, handlers);

				if (!L.NILP (handler_forms)) {
					Shelisp.Object rv = L.Qnil;
					foreach (var handler_form in (List)handler_forms)
						rv = handler_form.Eval(l);
					return rv;
				}

				// if nothing matches, rethrow
				throw;
			}
		}

		[LispBuiltin (Unevalled = true, DocString = @"Do BODYFORM, protecting with UNWINDFORMS.
If BODYFORM completes normally, its value is returned
after executing the UNWINDFORMS.
If BODYFORM exits nonlocally, the UNWINDFORMS are executed anyway.
usage: (unwind-protect BODYFORM UNWINDFORMS...)")]
		public static Shelisp.Object Funwind_protect (L l, Shelisp.Object bodyform, params Shelisp.Object[] unwindforms)
		{
			Shelisp.Object rv = L.Qnil;

			try {
				return bodyform.Eval(l);
			}
			finally {
				foreach (var unwind in unwindforms)
					unwind.Eval (l);
			}
		}

		[LispBuiltin]
		public static Shelisp.Object Fgetenv (L l, Shelisp.Object variable)
		{
			return (Shelisp.String)(System.Environment.GetEnvironmentVariable ((string)(Shelisp.String)variable) ?? "");
		}

		[LispBuiltin]
		public Shelisp.Object Vload_path = L.string_array_to_list (new string[] {"/Users/toshok/src/emacs/trunk/lisp", "/Users/toshok/src/emacs/trunk/lisp/emacs-lisp" });

		[LispBuiltin]
		public static Shelisp.Object Feval_when_compile (L l, params Shelisp.Object[] forms)
		{
			Shelisp.Object rv = L.Qnil;

			foreach (var o in forms)
				rv = o.Eval(l);
			return rv;
		}

		[LispBuiltin]
		public static Shelisp.Object Fintern (L l, Shelisp.Object symname, [LispOptional] Shelisp.Object obarray)
		{
			if (L.NILP(obarray)) obarray = L.current_obarray;

			Shelisp.Symbol s = L.intern ((string)(Shelisp.String)symname, (Vector)obarray);
			return s;
		}

		[LispBuiltin]
		public static Shelisp.Object Fintern_soft (L l, Shelisp.Object symname, [LispOptional] Shelisp.Object obarray)
		{
			if (L.NILP(obarray)) obarray = L.current_obarray;

			Shelisp.Object s = L.intern_soft ((string)(Shelisp.String)symname, (Vector)obarray);
			return s;
		}

		[LispBuiltin]
		public static Shelisp.Object Funintern (L l, Shelisp.Object symname, [LispOptional] Shelisp.Object obarray)
		{
			if (L.NILP(obarray)) obarray = L.current_obarray;

			return Obarray.Unintern ((Vector)obarray, (string)(Shelisp.String)symname);
		}

		[LispBuiltin]
		public Shelisp.Object Vpurify_flag = L.Qt;

		[LispBuiltin]
		public static Shelisp.Object Fcalled_interactively_p (L l, Shelisp.Object kind = null)
		{
			Console.WriteLine ("called-interactively-p not implemented");
			return L.Qnil;
		}

		[LispBuiltin (DocString = @"Return the current local time, as a human-readable string.
Programs can use this function to decode a time,
since the number of columns in each field is fixed
if the year is in the range 1000-9999.
The format is `Sun Sep 16 01:03:52 1973'.
However, see also the functions `decode-time' and `format-time-string'
which provide a much more powerful and general facility.

If SPECIFIED-TIME is given, it is a time to format instead of the
current time.  The argument should have the form (HIGH LOW . IGNORED).
Thus, you can use times obtained from `current-time' and from
`file-attributes'.  SPECIFIED-TIME can also have the form (HIGH . LOW),
but this is considered obsolete.")]
		public static Shelisp.Object Fcurrent_time_string (L l, [LispOptional] Shelisp.Object current_timestamp)
		{
			// XXX implement me...
			return (Shelisp.String)"hi";
		}

		[LispBuiltin (DocString = @"The value is a symbol indicating the type of operating system you are using.
Special values:
  `gnu'          compiled for a GNU Hurd system.
  `gnu/linux'    compiled for a GNU/Linux system.
  `gnu/kfreebsd' compiled for a GNU system with a FreeBSD kernel.
  `darwin'       compiled for Darwin (GNU-Darwin, Mac OS X, ...).
  `ms-dos'       compiled as an MS-DOS application.
  `windows-nt'   compiled as a native W32 application.
  `cygwin'       compiled using the Cygwin library.
Anything else (in Emacs 24.1, the possibilities are: aix, berkeley-unix,
hpux, irix, usg-unix-v) indicates some sort of Unix system.")]
		public Shelisp.Object Vsystem_type = L.get_system_type();

		static Shelisp.Object get_system_type ()
		{
			// XXX punt wow now
			return L.intern ("darwin");
		}

		[LispBuiltin]
		public static int max_specpdl_size = 1300; /* 1000 is not enough for CEDET's c-by.el.  */

		[LispBuiltin (DocString = @"*Non-nil means enter debugger if an error is signaled.
Does not apply to errors handled by `condition-case' or those
matched by `debug-ignored-errors'.
If the value is a list, an error only means to enter the debugger
if one of its condition symbols appears in the list.
When you evaluate an expression interactively, this variable
is temporarily non-nil if `eval-expression-debug-on-error' is non-nil.
The command `toggle-debug-on-error' toggles this.
See also the variable `debug-on-quit'.")]
		public static bool debug_on_error = false;


		[LispBuiltin (DocString = @"*List of errors for which the debugger should not be called.
Each element may be a condition-name or a regexp that matches error messages.
If any element applies to a given error, that error skips the debugger
and just returns to top level.
This overrides the variable `debug-on-error'.
It does not apply to errors handled by `condition-case'.")]
		public static bool debug_ignored_errors = false;

		[LispBuiltin (DocString = @"*Non-nil means enter debugger if quit is signaled (C-g, for example).
				     Does not apply if quit is handled by a `condition-case'.")]
		public static bool debug_on_quit = false;

		[LispBuiltin (DocString = @"Return result of expanding macros at top level of FORM.
If FORM is not a macro call, it is returned unchanged.
Otherwise, the macro is expanded and the expansion is considered
in place of FORM.  When a non-macro-call results, it is returned.

The second optional arg ENVIRONMENT specifies an environment of macro
definitions to shadow the loaded ones for use in file byte-compilation.")]
		public static Shelisp.Object Fmacroexpand (L l, Shelisp.Object form, Shelisp.Object environment)
		{
			/* With cleanups from Hallvard Furuseth.  */
			Shelisp.Object expander, sym, def, tem;

			while (true) {
				/* Come back here each time we expand a macro call,
				   in case it expands into another macro call.  */
				if (!L.CONSP (form))
					break;
				/* Set SYM, give DEF and TEM right values in case SYM is not a symbol. */
				def = sym = L.CAR (form);
				tem = L.Qnil;
				/* Trace symbols aliases to other symbols
				   until we get a symbol that is not an alias.  */
				while (def is Symbol) {
					//QUIT;
					sym = def;
					tem = List.Fassq (l, sym, environment);
					if (L.NILP (tem)) {
						def = ((Symbol)sym).Function;
						if (!def.LispEq (L.Qunbound))
							continue;
					}
					break;
				}
				/* Right now TEM is the result from SYM in ENVIRONMENT,
				   and if TEM is nil then DEF is SYM's function definition.  */
				if (L.NILP (tem)) {
					/* SYM is not mentioned in ENVIRONMENT.
					   Look at its function definition.  */
					if (def.LispEq (L.Qunbound) || !L.CONSP (def))
						/* Not defined or definition not suitable.  */
						break;
					if (L.Qautoload.LispEq(L.CAR (def))) {
						/* Autoloading function: will it be a macro when loaded?  */
						tem = List.Fnth (l, new Number (4), def);
						if (tem.LispEq (L.Qt) || tem.LispEq (L.Qmacro)) {
							FileIO.DoAutoload (l, def, sym);
							continue;
						}
						else
							break;
					}
					else if (!L.Qmacro.LispEq (L.CAR (def)))
						break;
					else
						expander = L.CDR (def);
				}
				else {
					expander = L.CDR (tem);
					if (L.NILP (expander))
						break;
				}
				form = new List (new Shelisp.Object[] { expander, L.CDR(form) }).Eval(l);
			}
			return form;
		}
	}
}

using System;
using System.Reflection;
using SysArray = System.Array;

namespace Shelisp {
	public class Subr : Object {
		public Subr (string name, string doc, int min_args, bool unevalled, MethodInfo method, object target = null)
		{
			this.name = name;
			this.doc = doc;
			this.min_args = min_args;
			this.unevalled = unevalled;
			this.method = method;
			this.target = target;
		}

		public string name;
		public string doc;
		public int min_args;
		public MethodInfo method;
		public object target;
		public bool unevalled;

		public Object Call (L l, Object[] args)
		{
			if (args.Length < min_args)
				throw new ArgumentException ("arg length");

			var parameters = method.GetParameters();

			var pattr = (ParamArrayAttribute)Attribute.GetCustomAttribute (parameters[parameters.Length-1], typeof (ParamArrayAttribute), true);
			bool has_params = pattr != null;

			int param_offset = has_params ? 2 : 1;

			object[] method_args = new object[parameters.Length];
			for (int i = 0; i < method_args.Length; i ++)
				method_args[i] = L.Qnil;

			method_args[0] = l;

			SysArray.Copy (args, 0, method_args, 1, Math.Min (args.Length, parameters.Length - param_offset));

			var num_rest_args = args.Length - parameters.Length + param_offset;
			if (num_rest_args > 0) {
				var rest_args = new Object[num_rest_args];

				SysArray.Copy (args, parameters.Length-param_offset, rest_args, 0, rest_args.Length);

				method_args[parameters.Length - 1] = rest_args;
			}
			else {
				if (has_params)
					method_args[parameters.Length - 1] = new Object[0];
			}

 			try {
				return (Object)method.Invoke (target, method_args);
 			}
			catch (ArgumentException e) {
				Console.WriteLine ("exception invoking {0}", method.Name);
 				foreach (var arg in method_args) {
					if (arg != null)
						Console.WriteLine (" +{0} {1}", arg.GetType(), arg);
				}
				Console.WriteLine (e);
				throw;
			}
			catch (TargetException e) {
				Console.WriteLine ("subr = {0}", this);
				Console.WriteLine ("exception {0}", e);
				throw;
			}
 			catch (TargetInvocationException e) {
#if DEBUG
 				Console.WriteLine ("Exception raised while invoking {0}", this);
				Console.WriteLine ("with parameters:");
				foreach (var p in parameters)
 					Console.WriteLine (" +{0} {1}", p.ParameterType, p.Name);
 				Console.WriteLine ("with args:");
 				foreach (var arg in method_args) {
					if (arg != null)
						Console.WriteLine (" +{0} {1}", arg.GetType(), arg);
				}
				Console.WriteLine (e);
#endif
 				throw e.InnerException;
 			}
		}

		public override string ToString (string format_type)
		{
#if VERBOSE_SUBR_FORMAT
			return string.Format ("#<subr {0},{1}.{2}()>", name, method.DeclaringType.FullName, method.Name);
#else
			return string.Format ("#<subr {0}>", name);
#endif
		}

		[LispBuiltin]
		public static Shelisp.Object Fsubrp(L l, Shelisp.Object subr)
		{
			return (subr is Subr) ? L.Qt : L.Qnil;
		}
	}

}
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
				var rv = (Object)method.Invoke (target, method_args);
				return rv;
			}
			catch (Exception e) {
				Console.WriteLine ("Exception raised while invoking {0}", this);
				Console.WriteLine ("with args:");
				foreach (var arg in args)
					Console.WriteLine (" + {0}", arg);
				Console.WriteLine (Environment.StackTrace);
				throw;
			}
		}

		public override string ToString ()
		{
#if VERBOSE_SUBR_FORMAT
			return string.Format ("#<subr {0},{1}.{2}()>", name, method.DeclaringType.FullName, method.Name);
#else
			return string.Format ("#<subr {0}>", name);
#endif
		}

		[LispBuiltin ("subrp", MinArgs = 1)]
		public static Shelisp.Object Fsubrp(L l, Shelisp.Object subr)
		{
			return (subr is Subr) ? L.Qt : L.Qnil;
		}
	}

}
namespace Shelisp {
	public class Quote : Object {
		public Quote (Object obj)
		{
			this.obj = obj;
		}

		public override Shelisp.Object Eval (L l, Shelisp.Object env = null)
		{
			return obj;
		}

		public override string ToString ()
		{
			return "(quote " + obj.ToString() + ")";
		}


		private Object obj;

		[LispBuiltin (Unevalled = true)]
		public static Shelisp.Object Fquote (L l, Shelisp.Object obj)
		{
			return obj;
		}
	}

}
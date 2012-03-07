namespace Lisp {
	class Quote : Object {
		public Quote (Object obj)
		{
			this.obj = obj;
		}

		public override Lisp.Object Eval (L l, Lisp.Object env = null)
		{
			return obj;
		}

		public override string ToString ()
		{
			return "(quote " + obj.ToString() + ")";
		}


		private Object obj;

		[LispBuiltin ("quote", MinArgs = 1, Unevalled = true)]
		public static Lisp.Object Fquote (L l, Lisp.Object obj)
		{
			return obj;
		}
	}

}
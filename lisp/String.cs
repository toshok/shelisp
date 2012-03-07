using System;
using System.Collections.Generic;
using System.Text;

namespace Lisp {
	public class String : Array {
		public String (string s)
		{
			this.native_string = new StringBuilder(s);
		}

		public StringBuilder native_string;

		public override int Length {
			get { return native_string.Length; }
		}

		public override Lisp.Object this[int index]
		{
			get { return native_string[index]; }
			set { /* XXX type check */ native_string[index] = (char)(int)(Number)value; }
		}

		public override IEnumerator<Lisp.Object> GetEnumerator ()
		{
			for (int i = 0; i < native_string.Length; i ++)
				yield return native_string[i];
		}

		public override string ToString ()
		{
			return string.Concat ("\"", native_string.ToString(), "\"");
		}

		public static implicit operator System.String (String str)
		{
			return str.native_string.ToString();
		}
	}

}
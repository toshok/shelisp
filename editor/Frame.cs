using Shelisp;

namespace Shemacs.Editor {
	class Frame : Object {

		public Frame (L l)
		{
		}

		public Shelisp.Object buffer_list;
		public Shelisp.Object buried_buffer_list;
	}

}
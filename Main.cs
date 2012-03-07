using System;
using System.IO;
using Lisp;

public class Shmacs {
	public static void Main (string[] args) {
		var l = new L();

		var list = L.make_list (1, 2, 3, "hi");

		Console.WriteLine (">>>>>>>> test 2");
		try {
			Console.WriteLine ("result = {0}", List.Fcar (l, L.make_list (1, 2, 3, 4, "hi")));
		}
		catch (Exception e) {
			Console.WriteLine ("failed {0}", e);
		}

		Console.WriteLine (">>>>>>>> test 3"); 
		try {
			Console.WriteLine ("result = {0}", L.make_list (L.Qcar, L.make_list (L.Qquote, list)).Eval(l));
		}
		catch (Exception e) {
			Console.WriteLine ("failed {0}", e);
		}

		Console.WriteLine (">>>>>>>> test 4"); 
		try {
			Lisp.Object obj = Lisp.Reader.Read ("(car '(1 2 3 \"hi\"))");
			Console.WriteLine ("result = {0}", obj.Eval(l));
		}
		catch (Exception e) {
			Console.WriteLine ("failed {0}", e);
		}

		Console.WriteLine (">>>>>>>> test 5"); 
		try {
			Lisp.Object obj = Lisp.Reader.Read ("((lambda (x) (car x)) '(1 2 3 \"hi\"))");
			Console.WriteLine ("result = {0}", obj.Eval(l));
		}
		catch (Exception e) {
			Console.WriteLine ("failed {0}", e);
		}

		Console.WriteLine (">>>>>>>> test 6"); 
		try {
			Lisp.Object obj = Lisp.Reader.Read ("(defun mycar (x) (car x))");
			obj.Eval(l);
			obj = Lisp.Reader.Read ("(mycar '(1 2 3 \"hi\"))");
			Console.WriteLine ("result = {0}", obj.Eval(l));
		}
		catch (Exception e) {
			Console.WriteLine ("failed {0}", e);
		}

		Console.WriteLine (">>>>>>>> test 999");
		try {
			Lisp.Object obj = Lisp.Reader.Read (new StreamReader ("test.lisp"));
			obj.Eval (l);
		}
		catch (Exception e) {
			Console.WriteLine ("failed {0}", e);
		}

		Console.WriteLine (">>>>>>>> test 9999"); 
		try {
			Lisp.Object obj = Lisp.Reader.Read (new StreamReader ("/Users/toshok/.emacs"));
			obj.Eval (l);
		}
		catch (Exception e) {
			Console.WriteLine ("failed {0}", e);
		}

#if false
		// (car 5), throws an exception
		Console.WriteLine (L.make_list (L.Qcar, 5).Eval(l));
#endif
	}
}
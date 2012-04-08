using System;
using System.IO;
using Shelisp;

using Shunit;

public class Test {
	public static void Main (string[] args) {
		L.RegisterGlobalBuiltins (typeof (TestRunner).Assembly);

		var l = new L();
#if false

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
			Shelisp.Object obj = Shelisp.Reader.Read ("(car '(1 2 3 \"hi\"))");
			Console.WriteLine ("result = {0}", obj.Eval(l));
		}
		catch (Exception e) {
			Console.WriteLine ("failed {0}", e);
		}

		Console.WriteLine (">>>>>>>> test 5"); 
		try {
			Shelisp.Object obj = Shelisp.Reader.Read ("((lambda (x) (car x)) '(1 2 3 \"hi\"))");
			Console.WriteLine ("result = {0}", obj.Eval(l));
		}
		catch (Exception e) {
			Console.WriteLine ("failed {0}", e);
		}

		Console.WriteLine (">>>>>>>> test 6"); 
		try {
			Shelisp.Object obj = Shelisp.Reader.Read ("(defun mycar (x) (car x))");
			obj.Eval(l);
			obj = Shelisp.Reader.Read ("(mycar '(1 2 3 \"hi\"))");
			Console.WriteLine ("result = {0}", obj.Eval(l));
		}
		catch (Exception e) {
			Console.WriteLine ("failed {0}", e);
		}

		Console.WriteLine (">>>>>>>> test 999");
		try {
			Shelisp.Object obj = Shelisp.Reader.Read (new StreamReader ("test.lisp"));
			obj.Eval (l);
		}
		catch (Exception e) {
			Console.WriteLine ("failed {0}", e);
		}

		Console.WriteLine (">>>>>>>> test 9999"); 
		try {
			Shelisp.Object obj = Shelisp.Reader.Read (new StreamReader ("/Users/toshok/.emacs"));
			obj.Eval (l);
		}
		catch (Exception e) {
			Console.WriteLine ("failed {0}", e);
		}

		// (car 5), throws an exception
		Console.WriteLine (L.make_list (L.Qcar, 5).Eval(l));
#endif

		TestRunner.Initialize (l, true);

		Number number = new Number (5);
		Assert.That (number, Is.Numberp, "make sure constructor works");
		Assert.That (new List (L.intern ("symbol-value"), new List (L.Qquote, L.intern ("huuuunh"))), Signals.Error, "void-variable");

		/* the rest of the tests come from .el files in the current directory */
		l.Vload_path = new List ((Shelisp.String)Environment.CurrentDirectory, l.Vload_path);

		FileIO.Fload_file (l, "run-tests.el");

		TestRunner.GenerateReport();
	}
}

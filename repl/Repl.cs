using System;
using System.IO;
using System.Threading;
using Shelisp;

public class Repl {
	public static void Main(string[] args)
	{
		L l = new L();

		object output_lock = new object ();
		int prompt_depth = 0;

		ReplStream stream = new ReplStream ();
		StreamReader repl_reader = stream.GetReader();
		StreamWriter repl_writer = stream.GetWriter();

		// start the reader in another thread so it can block waiting on more input
		new Thread (() => {
				while (true) {
					// this will block until the full form has been read by the reader
					var form = Shelisp.Reader.Read (repl_reader);
					Monitor.Enter (output_lock);
					try {
						var val = form.Eval (l);
						Console.WriteLine (" => {0}", val);
					}
					catch (Exception e) {
						Console.WriteLine ("error: {0}", e);
					}
					// once we've either thrown an exception or written output, reset the prompt depth
					prompt_depth = 0;
					Monitor.Pulse (output_lock);
					Monitor.Exit (output_lock);
				}
		}).Start();

		Monitor.Enter (output_lock);
		while (true) {
			Console.Write ("shelisp[{0}]> ", prompt_depth);
			for (int i = 0; i < prompt_depth; i ++)
				Console.Write ("  ");
			string input = Console.ReadLine ();
			if (input.Trim() != "")
				prompt_depth ++;
			repl_writer.WriteLine(input);
			repl_writer.Flush();

			// wait until evaluation is finished
			Monitor.Wait (output_lock, 100);
		}
	}
}
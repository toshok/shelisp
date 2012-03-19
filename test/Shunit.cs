using System;
using System.Linq;

using Shelisp;

namespace Shunit {

	public enum Is {
		Nil,
		T,
		Listp,
		Stringp,
		Numberp,
		Integerp,
		Floatp,
		Symbolp,
		Vectorp,
		Arrayp,
		Sequencep
	}

	public enum Signals {
		Error
	}

	public enum AssertResult {
		Failed,
		Succeeded
	}

	public static class TestRunner {
		public static void Initialize (L l, bool verbose)
		{
			L = l;
			Verbose = verbose;
		}

		public static L L { get; private set; }

		[LispBuiltin ("verbose-tests")]
		public static bool Verbose;

		[LispBuiltin ("number-of-tests")]
		public static int NumberOfTests = 0;

		[LispBuiltin ("failed-tests")]
		public static Shelisp.Object FailedTests = L.Qnil;

		public static void GenerateReport ()
		{
			Console.WriteLine ();
			Console.WriteLine ("ran {0} tests, {1} failures", NumberOfTests, L.NILP(FailedTests) ? 0 : ((List)FailedTests).Length);
		}
	}

	public static class Assert {
		public static AssertResult Succeed (string description)
		{
			Console.WriteLine ("SUCCEED");
			// record success + description
			return AssertResult.Succeeded;
		}

		public static AssertResult Fail (string description)
		{
			Console.WriteLine ("FAIL");
			// record failure + description
			return AssertResult.Failed;
		}

		public static AssertResult FailException (Exception e, string description)
		{
			Console.WriteLine ("FAILEX");
			Console.WriteLine (e);
			// record failure + description
			return AssertResult.Failed;
		}

		public static AssertResult FailExpectedLispError (string lisp_error, string description)
		{
			Console.WriteLine ("FAILEXPECTEDLISPERROR");
			// record failure + description
			return AssertResult.Failed;
		}

		public static AssertResult FailExpectedLispError (string lisp_error, LispException but_received, string description)
		{
			Console.WriteLine ("FAILEXPECTEDLISPERROR");
			Console.WriteLine (but_received);
			// record failure + description
			return AssertResult.Failed;
		}

		public static AssertResult That (Shelisp.Object o, Signals signals, string lisp_error, string description = null)
		{
			try {
				o.Eval (TestRunner.L);
				return FailExpectedLispError (lisp_error, description);
			}
			catch (LispException le) {
				if (le.Symbol == L.intern (lisp_error))
					return Succeed (description);
				else
					return FailExpectedLispError (lisp_error, le, description);
			}
			catch (Exception e) {
				return FailException (e, description);
			}
		}

		public static AssertResult That (Shelisp.Object @this, Is @is, string description = null)
		{
			switch (@is) {
			case Is.Nil:
				if (L.NILP(@this)) return Succeed (description);
				break;
			case Is.T:
				if (!L.NILP(@this)) return Succeed (description);
				break;
			case Is.Listp:
				if (@this is Shelisp.List) return Succeed (description);
				break;
			case Is.Stringp:
				if (@this is Shelisp.String) return Succeed (description);
				break;
			case Is.Numberp:
				if (@this is Number) return Succeed (description);
				break;
			case Is.Integerp:
				if (Number.IsInt(@this)) return Succeed (description);
				break;
			case Is.Floatp:
				if (Number.IsFloat(@this)) return Succeed (description);
				break;
			case Is.Symbolp:
				if (@this is Symbol) return Succeed (description);
				break;
			case Is.Vectorp:
				if (@this is Vector) return Succeed (description);
				break;
			case Is.Arrayp:
				if (@this is Shelisp.Array) return Succeed (description);
				break;
			case Is.Sequencep:
				if (@this is Sequence) return Succeed (description);
				break;
			default:
				return Fail ("unhandled Is value.");
			}

			return Fail (description);
		}

		public static AssertResult Eq (Shelisp.Object o1, Shelisp.Object o2, string description = null)
		{
			try {
				return (!o1.LispEq (o2)) ? Fail(description) : Succeed(description);
			}
			catch (Exception e) {
				return FailException (e, description);
			}
		}

		public static AssertResult NotEq (Shelisp.Object o1, Shelisp.Object o2, string description = null)
		{
			return (o1.LispEq (o2)) ? Fail(description) : Succeed(description);
		}

		public static AssertResult Equal (Shelisp.Object o1, Shelisp.Object o2, string description = null)
		{
			return (!o1.LispEqual (o2)) ? Fail(description) : Succeed(description);
		}

		public static AssertResult NotEqual (Shelisp.Object o1, Shelisp.Object o2, string description = null)
		{
			return (o1.LispEqual (o2)) ? Fail(description) : Succeed(description);
		}
	}

}

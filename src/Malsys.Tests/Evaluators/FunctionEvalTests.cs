// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Evaluators {
	[TestClass]
	public class FunctionEvalTests {

		[TestMethod]
		public void ConstantFunctionTests() {
			doTest("fun f() { return 0; }", "f()", "0");
			doTest("fun f() { return 0; } fun g() { return 1; }", "{f(), g()}", "{0, 1}");
		}

		[TestMethod]
		public void RecursionTests() {
			doTest("fun fib(x) { return if(x < 2, 1, fib(x - 1) + fib(x - 2)); }", "fib(8)", "34");
		}

		[TestMethod]
		public void FunCallInFunTests() {
			doTest("fun f() { return g(); } fun g() { return 5; }", "f()", "5");
		}

		[TestMethod]
		public void ParamsTests() {
			doTest("fun f(a, b) { return a + b; }", "f(1, 2)", "3");
			doTest("fun f(a, b, c = 3) { return a + b + c; }", "f(1, 0)", "4");
			doTest("fun f(a, b, c = 3) { return a + b + c; }", "f(1, 0, 2)", "3");
		}

		[TestMethod]
		public void LocalConstsTests() {
			doTest("fun f() { let x = 1; return x; }", "f()", "1");
			doTest("fun f(a) { let x = 1; return x + a; }", "f(1)", "2");
			doTest("fun f(a) { let x = a + 1; let y = x + 1; return y + 1; }", "f(1)", "4");
		}


		private void doTest(string inputDefinitions, string inputExpression, string excpected) {

			var inBlockEvaled = TestUtils.EvaluateInput(inputDefinitions);
			var result = TestUtils.EvaluateExpression(inputExpression, inBlockEvaled.ExpressionEvaluatorContext);
			string actual = TestUtils.Print(result);

			Assert.AreEqual(excpected, actual);

		}
	}
}

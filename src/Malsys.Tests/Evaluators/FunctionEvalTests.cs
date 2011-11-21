using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Malsys.Compilers;
using Malsys.Evaluators;
using Microsoft.FSharp.Text.Lexing;
using Malsys.Parsing;
using Malsys.IO;
using Malsys.SourceCode.Printers;

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


		private void doTest(string inputDefinitions, string inputExpression, string excpected) {

			var msgs = new MessagesCollection();
			var compiler = new InputCompiler(msgs);
			var inCompiled = compiler.CompileFromString(inputDefinitions, "TestDefs");

			if (msgs.ErrorOcured) {
				Assert.Fail("Failed to parse/compile definitions." + msgs.ToString());
			}

			var exprEvaluator = new ExpressionEvaluator();
			var evaluator = new InputEvaluator(exprEvaluator);
			var inBlockEvaled = evaluator.Evaluate(inCompiled);

			var lexBuff = LexBuffer<char>.FromString(inputExpression);
			var parsedExpr = ParserUtils.ParseExpression(lexBuff, msgs, "TestExprs");
			var compiledExpr = new ExpressionCompiler(msgs).CompileExpression(parsedExpr);

			if (msgs.ErrorOcured) {
				Assert.Fail("Failed to parse/compile expression." + msgs.ToString());
			}

			var result = new ExpressionEvaluator().Evaluate(compiledExpr, inBlockEvaled.GlobalConstants, inBlockEvaled.GlobalFunctions);

			var w = new IndentStringWriter();
			new CanonicPrinter(w).Print(result);

			Assert.AreEqual(excpected, w.GetResult());

		}
	}
}

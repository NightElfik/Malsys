using System;
using Malsys.Compilers;
using Malsys.IO;
using Malsys.Parsing;
using Malsys.SourceCode.Printers;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests {
	[TestClass]
	public class ExpressionCompilerTest {

		[TestMethod]
		public void ConstantTests() {
			doTest("0");
			doTest("3.14159");

			doTest("314159e-5", "3.14159");
			doTest("0.00314159e3", "3.14159");
			doTest("0.00314159e+3", "3.14159");

			doTest("0b1101");
			doTest("0B0011101", "0b11101");

			doTest("0o12345670");
			doTest("0O01234567", "0o1234567");

			doTest("0xabcdef", "0xABCDEF");
			doTest("0XABCDEF", "0xABCDEF");
			doTest("0x0A1B2c3d", "0xA1B2C3D");

			doTest("#0A1B2c3d", "#A1B2C3D");
		}

		[TestMethod]
		public void OperatorPrecedenceTests() {
			doTest("-1^2", "(-(1 ^ 2))");
			doTest("2 ^ -1", "(2 ^ (-1))");
			doTest("-1 * -1", "((-1) * (-1))");

			doTest("1+2*3^4", "(1 + (2 * (3 ^ 4)))");
			doTest("(1+2)*3^4", "((1 + 2) * (3 ^ 4))");
			doTest("((1+2)*3)^4", "(((1 + 2) * 3) ^ 4)");
		}


		private void doTest(string input, string excpected = null) {

			if (excpected == null) {
				excpected = input;
			}

			var lexBuff = LexBuffer<char>.FromString(input);
			var msgs = new MessageLogger();
			var expr = ParserUtils.ParseExpression(lexBuff, msgs, "testInput");
			var compiledExpr = new ExpressionCompiler(msgs).Compile(expr);

			var writer = new IndentStringWriter();
			var printer = new CanonicPrinter(writer);
			printer.Print(compiledExpr);

			string actual = writer.GetResult();

			if (msgs.ErrorOcured) {
				Console.WriteLine("in: " + input);
				Console.WriteLine("exc: " + excpected);
				Console.WriteLine("act: " + actual);
				Assert.Fail(msgs.ToString());
			}

			Assert.AreEqual(excpected, actual);
		}
	}
}

using System;
using Malsys.Compilers;
using Malsys.IO;
using Malsys.Parsing;
using Malsys.SourceCode.Printers;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Parsing {
	[TestClass]
	public class ExpressionTests {

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
		public void VariableTests() {
			doTest("var");
			doTest("v0ARiAbLE9");
			doTest(CharHelper.Pi.ToString());
			doTest("x'");
			doTest("_");
		}

		[TestMethod]
		public void OperatorTests() {
			doTest(" + 2");
			doTest(" - 0");
			doTest("0 + a");
			doTest("{5}[0] ^ 0");
			doTest(@"0 !$%&*+.\<>@^~?:-=/ a");
		}

		[TestMethod]
		public void FunCallTests() {
			doTest("f()");
			doTest("f'()");
			doTest("f_()");
			doTest("func(0)");
			doTest("func(x)");
			doTest("func(func(0))");
			doTest("func({})");
			doTest("func(a + 2)");
			doTest("func((2.1))");
			doTest("func({6}[0])");
			doTest("func(1, x, 0, f)");
		}

		[TestMethod]
		public void ArrayTests() {
			doTest("{}");
			doTest("{{{{}}}}");
			doTest("{{}, {{}, {{}}}, {}}");
			doTest("{0}");
			doTest("{a}");
			doTest("{func()}");
			doTest("{(3.3)}");
			doTest("{3 + 3}");
			doTest("{{9}[0]}");
			doTest("{1, 2, 3, a, b, c}");
		}

		[TestMethod]
		public void BracketsTests() {
			doTest("(2)");
			doTest("(((x)))");
			doTest("((2) ^ ((8 + 3) * a))");
			doTest("(func((5)))");
			doTest("(0 + a)");
			doTest("(r[0])");
		}

		[TestMethod]
		public void IndexerTests() {
			doTest("[2]");
			doTest("[x]");
			doTest("[0.3]");
			doTest("[func()]");
			doTest("[x + 1]");
			doTest("[x[5]]");
		}


		private void doTest(string input, string excpected = null) {

			if (excpected == null) {
				excpected = input;
			}

			var lexBuff = LexBuffer<char>.FromString(input);
			var msgs = new MessagesCollection();
			var expr = ParserUtils.ParseExpression(lexBuff, msgs, "testInput");

			var writer = new IndentStringWriter();
			new CanonicAstPrinter(writer).Print(expr);

			string actual = writer.GetResult().TrimEnd();

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

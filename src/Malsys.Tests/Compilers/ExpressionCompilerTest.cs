/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
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

			var compiledExpr = TestUtils.CompileExpression(input);
			string actual = TestUtils.Print(compiledExpr);

			Assert.AreEqual(excpected, actual);
		}
	}
}

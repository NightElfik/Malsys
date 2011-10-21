using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Malsys.Tests.Parsing {
	[TestClass]
	public class BasicLsystemTests {

		[TestMethod]
		public void EmptyInputTest() {
			testParser("");
		}

		[TestMethod]
		public void WhitespaceOnlyInputTest() {
			testParser("  \t\t\n  \t \r  \r\n\r  \t", "");
		}

		[TestMethod]
		public void GlobalVariableDefinitionTest() {
			testParser("let variable = 1;");
			testParser("let variable' = 2;");
			testParser("let {0} = 3.14159;".Fmt(CharHelper.Pi));
			testParser("let _var = -1;");
			testParser(new string[] { "let x=1;" }, "let x = 1;");
			testParser("let x = 1 + 2;");
			testParser(new string[] { "let x=0*0;" }, "let x = 0 * 0;");
			testParser("let arr = {1, log(e)}[1];");
		}

		[TestMethod]
		public void GlobalFunctionDefinitionTest() {
			testParser(new string[] { "fun f(){1}" }, "fun f() {", "\t1", "}");
			testParser("fun f'() {", "\tpi", "}");
			testParser("fun fun_x(x) {", "\tx", "}");
			testParser("fun fun_x(x) {", "\tx", "}");
		}


		private void testParser(params string[] input) {
			testParser(input, input);
		}

		private void testParser(string[] input, params string[] excpected) {

			string inputStr = string.Join(Environment.NewLine, input);
			string excpectedStr = string.Join(Environment.NewLine, excpected);

			string actual = ParsingTestUtils.ParseAndCanonicPrintAst(inputStr).TrimEnd();
			Assert.AreEqual(excpectedStr, actual);
		}

	}
}

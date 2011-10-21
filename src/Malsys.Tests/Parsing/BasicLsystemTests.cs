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




		private void testParser(params string[] input) {
			testParser(input, input);
		}

		private void testParser(string[] input, params string[] excpected) {

			string inputStr = string.Join(Environment.NewLine, input);
			string excpectedStr = string.Join(Environment.NewLine, excpected);

			string actual = ParsingTestUtils.ParseLsystemAndCanonicPrintAst(inputStr).TrimEnd();
			Assert.AreEqual(excpectedStr, actual);
		}

	}
}

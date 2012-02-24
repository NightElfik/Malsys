using System;
using System.Globalization;
using System.Threading;
using Malsys.IO;
using Malsys.Parsing;
using Malsys.SourceCode.Printers;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Parsing {
	[TestClass]
	public class VariableDefinitionTests {

		[TestInitialize]
		public void InitTest() {
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		}


		[TestMethod]
		public void VarNameTests() {
			doTest("let v=0;", "let v = 0;");
			doTest("let v = 0;");
			doTest("let var = 0;");
			doTest("let var' = 0;");
			doTest("let _ = 0;");
			doTest("let v_ = 0;");
			doTest("let _v = 0;");
			doTest("let {0} = 0;".Fmt(CharHelper.Pi));
		}

		[TestMethod]
		public void VarValueTests() {
			doTest("let v = 1;");
			doTest("let v =  - 1;");
			doTest("let x = 1 + 2;");
			doTest("let empty = {};");
			doTest("let arr = {1, log(e)}[1];");
			doTest("let v = f(x);");
		}


		private void doTest(string input, string excpected = null) {

			if (excpected == null) {
				excpected = input;
			}

			string actual = TestUtils.Print(TestUtils.ParseLsystem(input)).TrimEnd();
			Assert.AreEqual(excpected, actual);
		}
	}
}

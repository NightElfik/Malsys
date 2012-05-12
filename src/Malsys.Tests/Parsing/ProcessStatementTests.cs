/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Parsing {
	[TestClass]
	public class ProcessStatementTests {

		[TestMethod]
		public void BasicStatementTests() {
			doTestAutoident("process all with ConfigName;");
			doTestAutoident("process LsystemName with ConfigName;");
		}

		[TestMethod]
		public void UseAsStatementTests() {
			doTestAutoident("process LsystemName with ConfigName", "use SvgRewriter as Rewriter;");
			doTestAutoident("process LsystemName with ConfigName", "use SvgRewriter as Rewriter", "use NextComponent as AnotherContainerName;");
			doTestAutoident("process LsystemName with ConfigName", "use Fully.Qualified.Type as Rewriter;");
		}

		[TestMethod]
		public void LsysStatementsTests() {
			doTestAutoident("process LsystemName with ConfigName", "set a = 10;");
			doTestAutoident("process LsystemName with ConfigName", "set a = 10", "set symbols b = A B C(1, {2}, c);");
			doTestAutoident("process LsystemName with ConfigName", "rewrite a", "\tto b", "interpret a as b;");
			doTestAutoident("process LsystemName with ConfigName", "rewrite {a(x)} a(y) {a}", "\twhere x < y", "\tto b(a)", "interpret a b c(x, y = 10) as b(x + y);");
		}

		[TestMethod]
		public void LsystemArgsTests() {
			doTestAutoident("process all(a, r, g, s) with ConfigName;");
			doTestAutoident("process LsystemName(0, 1, {2}) with ConfigName;");
		}


		private void doTestAutoident(params string[] inputLines) {
			for (int i = 1; i < inputLines.Length; i++) {
				inputLines[i] = "\t" + inputLines[i];
			}

			string input = string.Join(Environment.NewLine, inputLines);
			doTest(input, input);
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

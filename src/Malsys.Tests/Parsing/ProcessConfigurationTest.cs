using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Parsing {
	[TestClass]
	public class ProcessConfigurationTest {

		[TestMethod]
		public void NoStatementsTests() {
			doTestAutoident("configuration ConfigName {", "}");
		}

		[TestMethod]
		public void ConponentsTests() {
			doTestAutoident("configuration ConfigName {", "component Interpret typeof SymbolsSaver;", "}");
			doTestAutoident("configuration ConfigName {", "component Interpret typeof Malsys.Components.SymbolsSaver;", "}");
		}

		[TestMethod]
		public void ContainersTests() {
			doTestAutoident("configuration ConfigName {", "container Rewriter typeof IRewriter default SymbolRewriter;", "}");
			doTestAutoident("configuration ConfigName {", "container Rewriter typeof Fully.Qualified.Type default Fully.Qualified.Type;", "}");
		}

		[TestMethod]
		public void ConnectionsTests() {
			doTestAutoident("configuration ConfigName {", "connect Iterator to Rewriter.OutputProcessor;", "}");
		}

		[TestMethod]
		public void VirtualConnectionsTests() {
			doTestAutoident("configuration ConfigName {", "virtual connect Iterator to Rewriter.OutputProcessor;", "}");
		}

		[TestMethod]
		public void AllStatementsTests() {
			doTestAutoident("configuration ConfigName {",
				"component Interpret typeof SymbolsSaver;",
				"container Rewriter typeof IRewriter default SymbolRewriter;",
				"container Iterator typeof IRewriterIterator default SingleRewriterIterator;",
				"connect Iterator to Rewriter.OutputProcessor;",
				"connect Rewriter to Iterator.Rewriter;",
				"connect Interpret to Iterator.OutputProcessor;",
				"}");
		}


		private void doTestAutoident(params string[] inputLines) {
			for (int i = 1; i < inputLines.Length - 1; i++) {
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

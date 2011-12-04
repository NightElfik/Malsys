using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.FSharp.Text.Lexing;
using Malsys.Parsing;
using Malsys.IO;
using Malsys.SourceCode.Printers;

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

			var lexBuff = LexBuffer<char>.FromString(input);
			var msgs = new MessagesCollection();
			var inBlock = ParserUtils.ParseInputNoComents(lexBuff, msgs, "testInput");

			var writer = new IndentStringWriter();
			new CanonicAstPrinter(writer).Print(inBlock);

			string actual = writer.GetResult().Trim();

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

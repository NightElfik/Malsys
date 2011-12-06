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
	public class ProcessStatementTests {

		[TestMethod]
		public void BasicStatementTests() {
			doTest("process with ConfigName;", "process this with ConfigName;");
			doTestAutoident("process this with ConfigName;");
			doTestAutoident("process LsystemName with ConfigName;");
		}

		[TestMethod]
		public void UseAsStatementTests() {
			doTestAutoident("process LsystemName with ConfigName", "use SvgRewriter as Rewriter;");
			doTestAutoident("process LsystemName with ConfigName", "use SvgRewriter as Rewriter", "use NextComponent as AnotherContainerName;");
			doTestAutoident("process LsystemName with ConfigName", "use Fully.Qualified.Type as Rewriter;");
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

			var lexBuff = LexBuffer<char>.FromString(input);
			var msgs = new MessageLogger();
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

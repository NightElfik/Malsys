using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.FSharp.Text.Lexing;
using Malsys.Compilers;
using Malsys.Parsing;
using Malsys.SourceCode.Printers;
using Malsys.IO;

namespace Malsys.Tests {
	[TestClass]
	public class ExpressionTests {

		[TestMethod]
		public void FloatConstantTests() {
			doTest("0");
			doTest("3.14159");

			doTest("0b0010");
			doTest("0B0011010010010");

			doTest("0o1234");
			doTest("0o5670");

			doTest("0xabcdef");
			doTest("0X123456");

			doTest("#abcdef0");
			doTest("#1234560");
		}

		[TestMethod]
		public void VariableTests() {
			doTest("x");
			doTest("x'");

			doTest("{0}".Fmt(CharHelper.Pi));
			doTest("{0}'".Fmt(CharHelper.Pi));

			doTest("_");
			doTest("_x");
			doTest("x_");

		}


		private void doTest(string input, string excpected = null) {

			if (excpected == null) {
				excpected = input;
			}

			var lexBuff = LexBuffer<char>.FromString(input);
			var msgs = new MessagesCollection();
			var expr = ParserUtils.ParseExpression(lexBuff, msgs, "testInput");

			if (msgs.ErrorOcured) {
				throw new Exception(msgs.ToString());
			}

			var writer = new IndentStringWriter();
			var printer = new CanonicAstPrinter(writer);
			printer.Visit(expr);

			string actual = writer.GetResult();

			Assert.AreEqual(excpected, actual);
		}
	}
}

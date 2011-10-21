using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.FSharp.Text.Lexing;
using Malsys.Compilers;
using Malsys.Parsing;
using Malsys.IO;
using Malsys.SourceCode.Printers;

namespace Malsys.Tests.Parsing {
	[TestClass]
	public class VariableDefinitionTests {

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
			doTest("let v = -1;");
			doTest("let x = 1+2;");
			doTest("let empty = {};");
			doTest("let arr = {1, log(e)}[1];");
			doTest("let v = f(x);");
		}


		private void doTest(string input, string excpected = null) {

			if (excpected == null) {
				excpected = input;
			}

			var lexBuff = LexBuffer<char>.FromString(input);
			var msgs = new MessagesCollection();
			var varDef = ParserUtils.ParseVarDef(lexBuff, msgs, "testInput");

			if (msgs.ErrorOcured) {
				throw new Exception(msgs.ToString());
			}

			var writer = new IndentStringWriter();
			var printer = new CanonicAstPrinter(writer);
			printer.Visit(varDef);

			string actual = writer.GetResult().TrimEnd();

			Assert.AreEqual(excpected, actual);
		}
	}
}

using System.Text;
using Malsys.Processing;
using Malsys.Processing.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Processing {
	[TestClass]
	public class ContextListBuilderTests {


		[TestMethod]
		public void EmptyInputTests() {
			doTest("");
		}

		[TestMethod]
		public void SymbolsOnlyTests() {
			doTest("A");
			doTest("A B C");
		}

		[TestMethod]
		public void ListOnlyTests() {
			doTest("[ ]");
			doTest("[ A B C ]");
			doTest("[ ] [ ] [ ]");
			doTest("[ A ] [ B ] [ C ]");
		}

		[TestMethod]
		public void ListInListTests() {
			doTest("A [ B ]");
			doTest("A [ B [ C ] ]");
			doTest("A [ B [ C [ D ] ] ]");
			doTest("[ [ [ A B ] ] ]");
			doTest("[ [ [ ] ] [ ] ]");
		}

		[TestMethod]
		public void MoreListInListTests() {
			doTest("A B [ C D ] E [ [ F ] G H ] I [ J ] [ K L [ [ ] M ] ] N O [ [ P ] Q R [ S ] T ] U V W [ X ] Y Z");
		}

		[TestMethod]
		[ExpectedException(typeof(ProcessException))]
		public void RedundantClose() {
			doTest("A [ B ] C ] D [ E");
		}




		private void doTest(string input) {
			doTest(input, input);
		}

		private void doTest(string input, string excpected) {

			var symbols = TestUtils.CompileSymbolsAsPattern(input);

			var builder = new ContextListBuilder<string>(s => s.Name == "[", s => s.Name == "]");

			foreach (var sym in symbols) {
				builder.AddSymbolToContext(sym);
			}

			var sb = new StringBuilder();
			contextNodeToString(builder.RootNode, sb);

			string actual = sb.ToString().Trim();
			excpected = excpected.Trim();
			if (excpected.Length == 0) {
				excpected = "[ ]";
			}
			else {
				excpected = "[ " + excpected + " ]";
			}

			Assert.AreEqual(excpected, actual);

		}


		private void contextNodeToString(ContextListNode<string> node, StringBuilder sb) {
			if (node.IsSymbolNode) {
				sb.Append(node.Symbol.Name);
				sb.Append(" ");
			}
			else {
				sb.Append("[ ");
				foreach (var n in node.InnerList) {
					contextNodeToString(n, sb);
				}
				sb.Append("] ");
			}
		}


	}
}

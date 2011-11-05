using Malsys.Rewriters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Rewriters {
	[TestClass]
	public class SymbolRewriterTests {

		[TestMethod]
		public void EmptyInputTests() {
			GenericRewriterTests.EmptyInputTests(new SymbolRewriter());
		}

		[TestMethod]
		public void NoRewriteRulesTests() {
			GenericRewriterTests.NoRewriteRulesTests(new SymbolRewriter());
		}

		[TestMethod]
		public void NothingOnRightSideTests() {
			GenericRewriterTests.NothingOnRightSideTests(new SymbolRewriter());
		}

		[TestMethod]
		public void PatternVarsTests() {
			GenericRewriterTests.PatternVarsTests(new SymbolRewriter());
		}

		[TestMethod]
		public void LeftContextTests() {
			GenericRewriterTests.LeftContextTests(new SymbolRewriter());
		}

		[TestMethod]
		public void RightContextTests() {
			GenericRewriterTests.RightContextTests(new SymbolRewriter());
		}

		[TestMethod]
		public void LocalVarsTests() {
			GenericRewriterTests.LocalVarsTests(new SymbolRewriter());
		}

		[TestMethod]
		public void ConditionTests() {
			GenericRewriterTests.ConditionTests(new SymbolRewriter());
		}

		[TestMethod]
		public void AnabaenaCatenulaTests() {
			GenericRewriterTests.AnabaenaCatenulaTests(new SymbolRewriter());
		}

		[TestMethod]
		public void SignalPropagationTests() {
			GenericRewriterTests.SignalPropagationTests(new SymbolRewriter());
		}
	}
}

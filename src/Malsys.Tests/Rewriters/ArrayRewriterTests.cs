using Malsys.Rewriters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Rewriters {
	[TestClass]
	public class ArrayRewriterTests {

		[TestMethod]
		public void EmptyInputTests() {
			GenericRewriterTests.EmptyInputTests(new ArrayRewriter());
		}

		[TestMethod]
		public void NoRewriteRulesTests() {
			GenericRewriterTests.NoRewriteRulesTests(new ArrayRewriter());
		}

		[TestMethod]
		public void NothingOnRightSideTests() {
			GenericRewriterTests.NothingOnRightSideTests(new ArrayRewriter());
		}

		[TestMethod]
		public void PatternVarsTests() {
			GenericRewriterTests.PatternVarsTests(new ArrayRewriter());
		}

		[TestMethod]
		public void LeftContextTests() {
			GenericRewriterTests.LeftContextTests(new ArrayRewriter());
		}

		[TestMethod]
		public void RightContextTests() {
			GenericRewriterTests.RightContextTests(new ArrayRewriter());
		}

		[TestMethod]
		public void LocalVarsTests() {
			GenericRewriterTests.LocalVarsTests(new ArrayRewriter());
		}

		[TestMethod]
		public void ConditionTests() {
			GenericRewriterTests.ConditionTests(new ArrayRewriter());
		}

		[TestMethod]
		public void AnabaenaCatenulaTests() {
			GenericRewriterTests.AnabaenaCatenulaTests(new ArrayRewriter());
		}

		[TestMethod]
		public void SignalPropagationTests() {
			GenericRewriterTests.SignalPropagationTests(new ArrayRewriter());
		}
	}
}

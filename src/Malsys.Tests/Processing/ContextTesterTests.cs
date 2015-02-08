using System;
using System.Linq;
using Malsys.Evaluators;
using Malsys.Processing.Context;
using Malsys.SemanticModel.Evaluated;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Processing {
	[TestClass]
	public class ContextTesterTests {

		// left context tests ==========================================================================================

		[TestMethod]
		public void EmptyLeftContextTests() {
			doLeftContextTest("S", "", true);
			doLeftContextTest("A B C S", "", true);
		}

		[TestMethod]
		public void SymbolsOnlyPositiveLeftContextTests() {
			doLeftContextTest("A S", "A", true);
			doLeftContextTest("B A S", "A", true);
			doLeftContextTest("C B A S", "C B A", true);
			doLeftContextTest("[ A S ]", "A", true);
		}

		[TestMethod]
		public void SymbolsOnlyNegativeLeftContextTests() {
			doLeftContextTest("A S", "B", false);
			doLeftContextTest("B A S", "B A A", false);
			doLeftContextTest("C B A S", "D C B A", false);
		}

		[TestMethod]
		public void ListOnlyPositiveLeftContextTests() {
			doLeftContextTest("[ A ] S", "[ A ]", true);
			doLeftContextTest("[ A B ] S", "[ A B ]", true);
		}

		[TestMethod]
		public void ListOnlyNegativeLeftContextTests() {
			doLeftContextTest("[ A ] S", "A", false);
			doLeftContextTest("A S", "[ A ]", false);
			doLeftContextTest("[ A ] S", "[ B A ]", false);
		}

		[TestMethod]
		public void PositiveLeftContextTests() {
			doLeftContextTest("C [ B ] A S", "[ B ] A", true);
			doLeftContextTest("X C [ B X Y ] A S", "C [ B ] A", true);
			doLeftContextTest("[ B X ] [ A X ] S", "[ B ] [ A ]", true);
		}

		[TestMethod]
		public void BranchesPositiveLeftContextTests() {
			doLeftContextTest("F [ C ] [ B ] [ A ] S", "F", true);
			doLeftContextTest("[ D ] [ C ] [ B ] A S", "[ C ] [ B ] [ D ] A", true);
			doLeftContextTest("X [ D X ] [ C X ] [ B X ] A S", "[ C ] [ B ] [ D ] A", true);
			doLeftContextTest("[ D ] [ C ] [ B ] S", "[ C ] [ D ]", true);

			doLeftContextTest("[ E ] [ D ] X [ C ] [ B ] [ A ] S", "[ E ] X [ B ]", true);
			doLeftContextTest("[ E Y ] [ D Y ] X [ C Y ] [ B Y ] [ A Y ] S", "[ E ] X [ B ]", true);

			doLeftContextTest("I [ H ] G [ F ] E [ D ] C [ B ] A S", "I G E C A", true);
			doLeftContextTest("I [ H ] G [ F ] E [ D ] C [ B ] A S", "I G [ F ] E C A", true);
		}

		[TestMethod]
		public void NegativeLeftContextTests() {
			doLeftContextTest("[ B ] A S", "[ B C ] A", false);
			doLeftContextTest("[ B D ] A S", "[ B C ] A", false);
			doLeftContextTest("C [ B ] [ X ] A S", "B A", false);
			doLeftContextTest("C [ B ] [ A ] S", "[ X ] C", false);
		}

		[TestMethod]
		public void ListInListPositiveLeftContextTests() {
			doLeftContextTest("F [ E ] [ D [ C ] ] [ B ] S", "F", true);

			doLeftContextTest("E [ D [ C ] B ] A S", "E [ D [ C ] B ] A", true);
			doLeftContextTest("X E [ D [ C X ] B X ] A S", "E [ D [ C ] B ] A", true);

			doLeftContextTest("X C [ [ B X ] X ] A S", "C [ [ B ] ] A", true);
			doLeftContextTest("X [ [ B X ] [ A X ] X ] S", "[ [ B ] [ A ] ]", true);
		}

		[TestMethod]
		public void ParentMatchSymbolPositiveLeftContextTests() {
			doLeftContextTest("A [ S ]", "A", true);
			doLeftContextTest("A [ [ S ] ]", "A", true);
			doLeftContextTest("X A [ [ X ] [ S ] ]", "A", true);
			doLeftContextTest("X A [ X ] [ [ S ] ]", "A", true);

			doLeftContextTest("C [ B [ A S ] ]", "C B A", true);
			doLeftContextTest("X C [ X ] [ B [ Y ] [ [ Z ] A [ W ] S ] ]", "C B A", true);
		}

		[TestMethod]
		public void ParentMatchListPositiveLeftContextTests() {
			doLeftContextTest("[ A ] [ S ]", "[ A ]", true);
			doLeftContextTest("[ A ] [ [ S ] ]", "[ A ]", true);
			doLeftContextTest("X [ A ] [ [ X ] [ S ] ]", "[ A ]", true);
			doLeftContextTest("X [ A X ] [ X ] [ [ S ] ]", "[ A ]", true);

			doLeftContextTest("[ C ] [ [ B ] [ [ A ] S ] ]", "[ C ] [ B ] [ A ]", true);
			doLeftContextTest("[ C X ] [ [ B X ] [ [ A X ] S ] ]", "[ C ] [ B ] [ A ]", true);

			doLeftContextTest("X [ E X ] [ F X ] [ [ D X ] [ C X ] [ [ A X ] [ B X ] S ] ]", "[ E ] [ D ] [ A ]", true);
		}

		[TestMethod]
		public void VariableMatchingLeftContextTests() {
			doLeftContextTest("B(2) A(1) S", "B(b) A(a)", true,
				"a = 1", "b = 2");

			doLeftContextTest("[ C(3) ] [ B(2) ] A S", "[ C(c) ] A", true,
				"c = 3");

			doLeftContextTest("[ X(5) [ B(2) ] [ C(3) ] Y(6) ] A S", "[ X(x) Y(y) ] A", true,
				"x = 5", "y = 6");

			doLeftContextTest("[ C(3) ] [ B(2) ] [ A(1) ] S", "[ B(b) ] [ C(c) ] [ A(a) ]", true,
				"a = 1", "b = 2", "c = 3");

			doLeftContextTest("X A(1) [ [ B(2) ] [ X ] [ S ] ]", "A(a) [ B(b) ]", true,
				"a = 1", "b = 2");

			doLeftContextTest("X [ E(5) X ] [ F(6) X ] [ [ D(4) X ] [ C(3) X ] [ [ A(1) X ] [ B(2) X ] S ] ]", "[ E(e) ] [ D(d) ] [ A(a) ]", true,
				"e = 5", "d = 4", "a = 1");
		}

		[TestMethod]
		public void ParentMatchNegativeLeftContextTests() {
			doLeftContextTest("X A [ X [ X ] [ S ] ]", "A", false);
			doLeftContextTest("[ A ] A [ [ S ] ]", "[ A ]", false);
		}

		[TestMethod]
		public void VariableMatchingNegativeLeftContextTests() {
			doLeftContextTest("C B(2) A(1) S", "D B(b) A(a)", false);
		}

		[TestMethod]
		public void VariableMatchingNotDefinedLeftContextTests() {
			doLeftContextTest("B(2, 3) A(1) S", "B(b1, b2) A(a1, a2)", true,
				"a1 = 1", "a2 = NaN", "b1 = 2", "b2 = 3");
		}

		[TestMethod]
		public void MultipleSameBranchesLeftContextTests() {
			doLeftContextTest("[ B(2) ] [ B(3) ] S", "[ B(a) ] [ B(b) ]", true,
				"a = 2", "b = 3");

			doLeftContextTest("[ B(2) ] [ X(5) ] [ B(3) ] S", "[ B(a) ] [ B(b) ]", true,
				"a = 2", "b = 3");

			doLeftContextTest("X [ [ B(2) ] [ X(5) ] [ B(3) ] X ] S", "[ [ B(a) ] [ B(b) ] ]", true,
				"a = 2", "b = 3");
		}


		// right context tests ==========================================================================================

		[TestMethod]
		public void EmptyRightContextTests() {
			doRightContextTest("", "", true);
			doRightContextTest("A B C", "", true);
		}

		[TestMethod]
		public void SymbolsOnlyPositiveRightContextTests() {
			doRightContextTest("A", "A", true);
			doRightContextTest("A B", "A", true);
			doRightContextTest("A B C", "A B C", true);
		}

		[TestMethod]
		public void SymbolsOnlyNegativeRightContextTests() {
			doRightContextTest("A", "B", false);
			doRightContextTest("A B", "A A B", false);
			doRightContextTest("A B C", "A B C D", false);
		}

		[TestMethod]
		public void ListOnlyPositiveRightContextTests() {
			doRightContextTest("[ A ]", "[ A ]", true);
			doRightContextTest("[ A B ]", "[ A B ]", true);
		}

		[TestMethod]
		public void ListOnlyNegativeRightContextTests() {
			doRightContextTest("[ A ]", "A", false);
			doRightContextTest("A", "[ A ]", false);
			doRightContextTest("[ A ]", "[ A B ]", false);
		}

		[TestMethod]
		public void PositiveRightContextTests() {
			doRightContextTest("A [ B ] C", "A [ B ]", true);
			doRightContextTest("A [ B X Y ] C", "A [ B ] C", true);
			doRightContextTest("[ A X ] [ B X ]", "[ A ] [ B ]", true);
		}

		[TestMethod]
		public void BranchesPositiveRightContextTests() {
			doRightContextTest("[ B ] [ C ] [ D ] F", "F", true);
			doRightContextTest("A [ B ] [ C ] [ D ]", "A [ C ] [ B ] [ D ]", true);
			doRightContextTest("A [ B X ] [ C X ] [ D X ] X", "A [ C ] [ B ] [ D ]", true);
			doRightContextTest("A [ B ] [ C ] [ D ]", "A [ C ] [ D ]", true);

			doRightContextTest("[ B ] [ C ] D [ E ] [ F ]", "[ B ] D [ F ]", true);
			doRightContextTest("[ B X ] [ C X ] D [ E X ] [ F X ]", "[ B ] D [ F ]", true);

			doRightContextTest("A [ B ] C [ D ] E [ E ] F [ G ] H", "A C E F H", true);
			doRightContextTest("A [ B ] C [ D ] [ W ] E [ F ] G", "A C [ W ] E G", true);
		}

		[TestMethod]
		public void NegativeRightContextTests() {
			doRightContextTest("A [ B ]", "A [ B C ]", false);
			doRightContextTest("A [ B D ]", "A [ B C ]", false);
			doRightContextTest("A [ B ] [ X ] C", "A B", false);
			doRightContextTest("[ B ] [ A ] C", "[ C ] C", false);
		}

		[TestMethod]
		public void ListInListPositiveRightContextTests() {
			doRightContextTest("[ B ] [ C [ X ] ] [ D ] F", "F", true);

			doRightContextTest("A [ B [ C ] D ] E", "A [ B [ C ] D ] E", true);
			doRightContextTest("A [ B [ C X ] D X ] E X", "A [ B [ C ] D ] E", true);

			doRightContextTest("A [ [ B X ] X ] C X", "A [ [ B ] ] C", true);
			doRightContextTest("[ [ A X ] [ B X ] X ] X", "[ [ A ] [ B ] ]", true);
		}

		[TestMethod]
		public void VariableMatchingRightContextTests() {
			doRightContextTest("A(1) B(2)", "A(a) B(b)", true,
				"a = 1", "b = 2");

			doRightContextTest("A [ B(2) ] [ C(3) ]", "A [ C(c) ]", true,
				"c = 3");

			doRightContextTest("A [ X(5) [ B(2) ] [ C(3) ] Y(6) ]", "A [ X(x) Y(y) ]", true,
				"x = 5", "y = 6");

			doRightContextTest("[ A(1) ] [ B(2) ] [ C(3) ]", "[ B(b) ] [ C(c) ] [ A(a) ]", true,
				"a = 1", "b = 2", "c = 3");
		}

		[TestMethod]
		public void VariableMatchingNegativeRightContextTests() {
			doRightContextTest("A(1) B(2) C", "A(a) B(b) D", false);
		}

		[TestMethod]
		public void VariableMatchingNotDefinedRightContextTests() {
			doRightContextTest("A(1) B(2, 3)", "A(a1, a2) B(b1, b2)", true,
				"a1 = 1", "a2 = NaN", "b1 = 2", "b2 = 3");
		}

		[TestMethod]
		public void MultipleSameBranchesRightContextTests() {
			doRightContextTest("[ B(2) ] [ B(3) ]", "[ B(a) ] [ B(b) ]", true,
				"a = 2", "b = 3");

			doRightContextTest("[ B(2) ] [ X(5) ] [ B(3) ]", "[ B(a) ] [ B(b) ]", true,
				"a = 2", "b = 3");

			doRightContextTest("[ [ B(2) ] [ X(5) ] [ B(3) ] X ] X", "[ [ B(a) ] [ B(b) ] ]", true,
				"a = 2", "b = 3");
		}



		/// <summary>
		/// Last symbol is considered as symbol from which is left context checked.
		/// </summary>
		public void doLeftContextTest(string inputSymbolsStr, string contextSymbolsStr, bool excpectedResult, params string[] variables) {
			doTest(inputSymbolsStr, contextSymbolsStr, excpectedResult, variables, true);
		}

		/// <summary>
		/// Adds first symbol from which is right context checked.
		/// </summary>
		public void doRightContextTest(string inputSymbolsStr, string contextSymbolsStr, bool excpectedResult, params string[] variables) {
			doTest("S " + inputSymbolsStr, contextSymbolsStr, excpectedResult, variables, false);
		}

		public void doTest(string inputSymbolsStr, string contextSymbolsStr, bool excpectedResult, string[] excpectedVariables, bool leftContext) {

			var intputSymbolsEvaled = TestUtils.EvaluateSymbols(TestUtils.CompileSymbols(inputSymbolsStr));
			var inputNodeList = intputSymbolsEvaled
				.Aggregate(new ContextListBuilder<IValue>(s => s.Name == "[", s => s.Name == "]"),
					(builder, sym) => { builder.AddSymbolToContext(sym); return builder; })
				.RootNode.InnerList;

			var contextSymbols = TestUtils.CompileSymbolsAsPattern(contextSymbolsStr);
			var context = contextSymbols
				.Aggregate(new ContextListBuilder<string>(s => s.Name == "[", s => s.Name == "]"),
					(builder, sym) => { builder.AddSymbolToContext(sym); return builder; })
				.RootNode.InnerList;


			var checker = new ContextChecker();

			IExpressionEvaluatorContext eec = new ExpressionEvaluatorContext();
			bool actualResult;

			if (leftContext) {
				while (inputNodeList.Last.IsListNode) {
					// find last symbol
					inputNodeList = inputNodeList.Last.InnerList;
				}
				Assert.AreEqual(inputNodeList.Last.Symbol.Name, "S", "Start symbol of left context checking must be `S` to avoid mistakes in tests.");
				actualResult = checker.CheckLeftContextOfSymbol(inputNodeList.Last, context, ref eec);
			}
			else {
				Assert.AreEqual(inputNodeList.First.Symbol.Name, "S", "Start symbol of right context checking must be `S` to avoid mistakes in tests.");
				actualResult = checker.CheckRightContextOfSymbol(inputNodeList.First, context, ref eec);
			}

			var actualVariables = eec.GetAllStoredVariables().Select(v => v.Name + " = " + TestUtils.Print(v.ValueFunc())).ToList();

			Console.WriteLine("Input: " + inputSymbolsStr);
			Console.WriteLine("Context: " + contextSymbolsStr);
			Assert.AreEqual(excpectedResult, actualResult);
			CollectionAssert.AreEquivalent(excpectedVariables, actualVariables);
		}
	}
}

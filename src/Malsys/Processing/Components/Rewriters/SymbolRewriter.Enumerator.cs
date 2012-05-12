/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Malsys.Evaluators;
using Malsys.Processing.Context;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Symbol = Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>;
using SymbolPatern = Malsys.SemanticModel.Symbol<string>;

namespace Malsys.Processing.Components.Rewriters {
	public partial class SymbolRewriter {

		/// <remarks>
		/// Enumerator is reusable BUT Reset method must be called before each usage.
		/// Even source (symbol provider) can be switched between usages, Reset call will gets new enumerator.
		/// </remarks>
		private class SymbolRewriterEnumerator : IEnumerator<Symbol> {

			private const string randomFuncName = "random";

			private static readonly ContextChecker contextChecker = new ContextChecker();
			private static readonly IValue[] emptyArgs = new IValue[0];


			private readonly SymbolRewriter parent;
			private readonly IMessageLogger logger;
			private readonly IExpressionEvaluatorContext exprEvalCtxt;
			private readonly Dictionary<string, RewriterRewriteRule[]> rewriteRules;
			private readonly HashSet<string> contextIgnoredSymbolNames;
			private readonly HashSet<string> contextSymbols;

			private IEnumerator<Symbol> symbolsSource;

			private readonly IndexableQueue<Symbol> inputBuffer;

			private readonly IndexableQueue<Symbol> outputBuffer;

			/// <summary>
			/// Maximal length of left context from all rewrite rules.
			/// </summary>
			private readonly int leftCtxtMaxLen;

			/// <summary>
			/// Maximal length of right context from all rewrite rules.
			/// </summary>
			private readonly int rightCtxtMaxLen;

			private ContextListNode<IValue> contextRootNode;
			private ContextListNode<IValue> currentSombolInContext;

			private readonly ContextListBuilder<IValue> contextBuilder;

			private Random emergencyRandomGenerator = null;



			public SymbolRewriterEnumerator(SymbolRewriter parentSr) {

				parent = parentSr;
				logger = parent.Logger;

				rewriteRules = parent.rewriteRules;
				exprEvalCtxt = parent.exprEvalCtxt;

				contextIgnoredSymbolNames = parent.contextIgnoredSymbolNames;
				contextSymbols = parent.contextSymbols;

				leftCtxtMaxLen = parent.leftCtxtMaxLen;
				rightCtxtMaxLen = parent.rightCtxtMaxLen;

				inputBuffer = new IndexableQueue<Symbol>(rightCtxtMaxLen + 4);
				// optimal output buffer length is max length of any replacement
				outputBuffer = new IndexableQueue<Symbol>(parent.rrReplacementMaxLen + 1);

				contextBuilder = new ContextListBuilder<IValue>(s => parent.startBranchSymbolNames.Contains(s.Name), s => parent.endBranchSymbolNames.Contains(s.Name));

			}


			#region IEnumerator<Symbol<IValue>> Members

			public bool MoveNext() {

				if (outputBuffer.Count > 0) {
					Current = outputBuffer.Dequeue();
					return true;
				}

				Symbol currSymbol;
				while (tryGetInputSymbol(out currSymbol)) {

					rewrite(currSymbol);

					// TODO: clean context from left

					if (outputBuffer.Count > 0) {
						Current = outputBuffer.Dequeue();
						return true;
					}
				}

				Current = null;
				return false;  // no symbols to process
			}

			public Symbol Current { get; private set; }

			object System.Collections.IEnumerator.Current {
				get { return Current; }
			}

			/// <summary>
			/// Must be called before any usage, even before first usage.
			/// </summary>
			public void Reset() {

				symbolsSource = parent.SymbolProvider.GetEnumerator();
				inputBuffer.Clear();
				outputBuffer.Clear();
				contextBuilder.Reset();
				contextRootNode = contextBuilder.RootNode;
				currentSombolInContext = contextRootNode;  // will correctly set to symbol after load (next of list is its first item)

			}

			public void Dispose() {
				// do not dispose anything since enumerator can be reused
			}

			#endregion


			private bool tryGetInputSymbol(out Symbol symbol) {

				if (!ensureInputSymbolsCount(1)) {
					symbol = null;
					return false;
				}

				symbol = inputBuffer.Dequeue();
				if (!contextSymbols.Contains(symbol.Name)) {
					// search for symbol in hierarchy should never fail because it is done after non-ignored symbol is
					// loaded thus it must be in context
					if (currentSombolInContext.GetNextSymbolNodeInHierarchy() == null) {
						var i = 10;
						i++;
					}
					currentSombolInContext = currentSombolInContext.GetNextSymbolNodeInHierarchy();
					Debug.Assert(currentSombolInContext.Symbol == symbol, "Context is not synchronized.");
				}

				return true;
			}

			private bool ensureInputSymbolsCount(int count) {

				Contract.Ensures(Contract.Result<bool>() ? inputBuffer.Count >= count : true);

				int symbolsToLoad = count - inputBuffer.Count;

				if (symbolsToLoad <= 0) {
					return true;  // required amount of symbols already loaded
				}

				int loaded = loadSymbols(symbolsToLoad);
				return loaded >= symbolsToLoad; // return false if less than required is loaded
			}

			private int loadSymbols(int count) {

				int loadedSymbolsCount = 0;

				while (count > 0 && symbolsSource.MoveNext()) {
					var symbol = symbolsSource.Current;
					inputBuffer.Enqueue(symbol);
					if (!contextIgnoredSymbolNames.Contains(symbol.Name)) {
						contextBuilder.AddSymbolToContext(symbol);
					}
					loadedSymbolsCount++;
					count--;
				}

				return loadedSymbolsCount;

			}



			/// <summary>
			/// Rewrites given symbol to output buffer.
			/// </summary>
			private void rewrite(Symbol symbol) {

				IExpressionEvaluatorContext eec;
				RewriterRewriteRule rrule;

				if (tryFindRewriteRule(symbol, out rrule, out eec)) {

					var replac = chooseReplacement(rrule, eec);

					foreach (var replacSymbol in replac.Replacement) {
						var evaledSymbol = new Symbol<IValue>(replacSymbol.Name, eec.EvaluateList(replacSymbol.Arguments));
						outputBuffer.Enqueue(evaledSymbol);
					}
				}
				else {
					// implicit identity
					outputBuffer.Enqueue(symbol);
				}
			}

			private bool tryFindRewriteRule(Symbol symbol, out RewriterRewriteRule rruleResult, out IExpressionEvaluatorContext empResult) {

				RewriterRewriteRule[] rrules;
				if (rewriteRules.TryGetValue(symbol.Name, out rrules)) {
					foreach (var rr in rrules) {

						Debug.Assert(rr.SymbolPattern.Name == symbol.Name, "Bad rewrite rule Dictionary.");

						// get work copy of variables
						var eec = exprEvalCtxt;

						// left context check first, it is simpler (do not invoke input reading as right context check)
						if (!rr.LeftContext.IsEmpty && !contextChecker.CheckLeftContextOfSymbol(currentSombolInContext, rr.LeftContext, ref eec)) {
							continue;
						}

						if (!rr.RightContext.IsEmpty) {
							ensureEnoughSymbolsForRightContext(rr.RightContextLenth);
							if (!contextChecker.CheckRightContextOfSymbol(currentSombolInContext, rr.RightContext, ref eec)) {
								continue;
							}
						}

						// map pattern
						mapPatternConsts(rr.SymbolPattern, symbol, ref eec);

						// map local constant definitions
						foreach (var cd in rr.LocalConstantDefs) {
							eec = eec.AddVariable(cd.Name, eec.Evaluate(cd.Value));
						}

						// check condition
						var condValue = eec.Evaluate(rr.Condition);
						if (!condValue.IsConstant) {
							continue;
						}

						var condConst = (Constant)condValue;
						if (condConst.IsNaN || condConst.IsZero) {
							continue;
						}

						rruleResult = rr;
						empResult = eec;


						return true;
					}

				}

				rruleResult = null;
				empResult = null;
				return false;
			}




			private void ensureEnoughSymbolsForRightContext(int nodesToLoad) {

				var prevNode = currentSombolInContext;

				while (nodesToLoad >= 0) {  // load one more than desired

					if (prevNode.Next == null) {
						if (loadSymbols(4 + nodesToLoad * 2) == 0) {
							return;  // no more symbols to load
						}
					}
					else {
						prevNode = prevNode.Next;
						nodesToLoad--;
					}

				}

			}

			private void mapPatternConsts(SymbolPatern pattern, Symbol symbol, ref IExpressionEvaluatorContext eec) {

				int paramsLen = symbol.Arguments.Length;
				int patternLen = pattern.Arguments.Length;

				for (int i = 0; i < patternLen; i++) {
					// set value to NaN if symbol has not enough actual parameters to match pattern
					var value = i < paramsLen ? symbol.Arguments[i] : Constant.NaN;
					eec = eec.AddVariable(pattern.Arguments[i], value);
				}

			}

			private RewriteRuleReplacement chooseReplacement(RewriterRewriteRule rr, IExpressionEvaluatorContext eec) {

				int rrrLen = rr.Replacements.Count;

				if (rrrLen == 0) {
					return RewriteRuleReplacement.Empty;
				}
				else if (rrrLen == 1) {
					return rr.Replacements[0];
				}

				// usage of Math.Max to allow only non-negative values
				var weights = rr.Replacements.Select(replac => Math.Max(0, (double)eec.EvaluateAsConst(replac.Weight))).ToArray();
				double sumWeights = weights.Sum();
				double rand = nextRandom() * sumWeights;
				double acc = 0d;

				for (int i = 0; i < rrrLen; i++) {
					acc += weights[i];
					if (rand < acc) {
						return rr.Replacements[i];
					}
				}

				// this should be unreachable but float arithmetic may be inaccurate so if program reaches here,
				// acc should be very close to rand
				// check this invariant while debugging
				Debug.Assert(Math.Abs(rand - acc) < 1e-6, "Random number {0} should be close to {1}.".Fmt(rand, acc));

				// return last item as result
				return rr.Replacements[rrrLen - 1];
			}

			private double nextRandom() {

				if (emergencyRandomGenerator != null) {
					return emergencyRandomGenerator.NextDouble();
				}

				IValue result;
				if (parent.context.ExpressionEvaluatorContext.TryEvaluateFunction(randomFuncName, emptyArgs, out result)) {
					if (result.IsConstant) {
						return ((Constant)result).Value;
					}
				}

				logger.LogMessage(Message.NoRandomFunc, randomFuncName);

				emergencyRandomGenerator = new Random();
				return emergencyRandomGenerator.NextDouble();

			}

		}

	}
}

/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using Malsys.Evaluators;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using System.Threading;

namespace Malsys.Processing.Context {
	/// <summary>
	/// All public members are thread safe.
	/// </summary>
	/// <remarks>
	/// This class has no internal state. All methods are "pure" functions thus they are thread safe.
	/// This class is not static because of testing, deriving or future extensions.
	/// </remarks>
	public class ContextChecker {

		private int contextSearchIdCounter = 0;

		/// <summary>
		/// Checks left context of given symbol with respect to branches.
		/// It also saves matched variables to given EEC if true is returned.
		/// </summary>
		/// <param name="symbolNode">Symbol which context will be checked.</param>
		/// <param name="context">List of context symbols.</param>
		/// <param name="resultEec">EEC where matched variables will be saved. It context checking failed, no variables are saved (EEC remains unchanged).</param>
		/// <returns>True if given context matches symbol's left context, false otherwise.</returns>
		public bool CheckLeftContextOfSymbol(ContextListNode<IValue> symbolNode, ContextListNode<string>.ContextList context, ref IExpressionEvaluatorContext resultEec) {

			int searchId = Interlocked.Increment(ref contextSearchIdCounter);  // to preserve thread safety
			return checkLeftContextOfSymbol(symbolNode, context.Last, searchId, ref resultEec);

		}

		/// <summary>
		/// Checks right context of given symbol with respect to branches.
		/// It also saves matched variables to given EEC if true is returned.
		/// </summary>
		/// <param name="symbolNode">Symbol which context will be checked.</param>
		/// <param name="context">List of context symbols.</param>
		/// <param name="resultEec">EEC where matched variables will be saved. It context checking failed, no variables are saved (EEC remains unchanged).</param>
		/// <returns>True if given context matches symbol's left context, false otherwise.</returns>
		public bool CheckRightContextOfSymbol(ContextListNode<IValue> symbolNode, ContextListNode<string>.ContextList context, ref IExpressionEvaluatorContext resultEec) {

			int searchId = Interlocked.Increment(ref contextSearchIdCounter);  // to preserve thread safety
			return CheckRightContextOfSymbol(symbolNode.Next, context.First, searchId, ref resultEec);

		}

		/// <summary>
		/// Checks if given left context matches to given symbol's context with respect to branches.
		/// It also saves matched variables to given EEC if true is returned.
		/// </summary>
		/// <param name="symbolNode">Symbol which context will be checked.</param>
		/// <param name="lastContextNode">Last (right) node in list of left context. From this node to previous nodes is context checked.</param>
		/// <param name="contextSearchId">ID for distinguishing matched branches.</param>
		/// <param name="resultEec">EEC where matched variables will be saved. It context checking failed, no variables are saved (EEC remains unchanged).</param>
		/// <returns>True if given context matches symbol's left context, false otherwise.</returns>
		private bool checkLeftContextOfSymbol(ContextListNode<IValue> symbolNode, ContextListNode<string> lastContextNode,
				int contextSearchId, ref IExpressionEvaluatorContext resultEec) {

			var eec = resultEec;
			// already checked node, previous node is checked
			// we need to keep reference to parent so we have to work with previous node (current can be null => lost reference)
			var lastNode = symbolNode;

			// for each node in context array (from right to left)
			for (var ctxtNode = lastContextNode; ctxtNode != null; ctxtNode = ctxtNode.Previous) {
				if (ctxtNode.IsSymbolNode) {

					var node = lastNode.Previous;
					// skip all branches (non-symbol nodes)
					// A < S must match in "A S" and also in "A [x] [y] S"
					while (node != null && !node.IsSymbolNode) {
						node = node.Previous;
					}

					if (node == null) {
						// we searched till end of array, lets look in in parent
						// A < S must match in "A [ S ..." and also in "A [x] [y] [ S ..."
						if (lastNode.ParentNode == null) {
							return false;  // we have no parent
						}
						if (checkLeftContextOfSymbol(lastNode.ParentNode, ctxtNode, contextSearchId, ref eec)) {
							// recursive call matched rest of context
							resultEec = eec;  // return EEC only if context is matched successfully
							return true;
						}
						else {
							return false;  // matching symbol not found
						}
					}
					else if (node.Symbol.Name != ctxtNode.Symbol.Name) {
						return false;  // name do not match context
					}

					eec = mapPatternConsts(ctxtNode.Symbol, node.Symbol, eec);
					lastNode = node;  // move last node
				}
				else {
					// try to match context on all precedent branches
					// [A] < S must match in "[A] S" and also in "[A] [x] [y] S"
					ContextListNode<IValue> matchedNode = null;
					var node = lastNode.Previous;
					for (; node != null && node.IsListNode; node = node.Previous) {
						if (CheckRightContextOfSymbol(node.InnerList.First, ctxtNode.InnerList.First, contextSearchId, ref eec)) {
							if (node.MatchedId == contextSearchId) {
								continue;  // this node was already matched
							}

							node.MatchedId = contextSearchId;  // mark node as matched
							matchedNode = node;
							break;
						}
					}

					if (matchedNode == null) {
						if (node != null) {
							return false;  // we found symbol node
						}
						// we searched till end of array, lets look in in parent
						// [A] < S must match in "[A] [ S ..." and also in "[A] [x] [y] [ S ..."
						if (lastNode.ParentNode == null) {
							return false;  // we have no parent
						}
						if (checkLeftContextOfSymbol(lastNode.ParentNode, ctxtNode, contextSearchId, ref eec)) {
							// recursive call matched rest of context
							resultEec = eec;  // return EEC only if context is matched successfully
							return true;
						}
						else {
							return false;  // matching list not found
						}
					}
					// do not move current node
				}
			}

			resultEec = eec;  // return EEC only if context is matched successfully
			return true;
		}


		/// <summary>
		/// Checks if given right context matches to symbols in given list with respect to branches.
		/// It also saves matched variables to given EEC if true is returned.
		/// </summary>
		/// <param name="firstNodeAfterSymbol">First node after symbol which context is checked.</param>
		/// <param name="firstNodeOfContext">First (left) node in list of right context. From this node to next nodes is context checked.</param>
		/// <param name="contextSearchId">ID for distinguishing matched branches.</param>
		/// <param name="resultEec">EEC where matched variables will be saved. It context checking failed, no variables are saved (EEC remains unchanged).</param>
		/// <returns>True if given context matches symbols right of given start node, false otherwise.</returns>
		public bool CheckRightContextOfSymbol(ContextListNode<IValue> firstNodeAfterSymbol, ContextListNode<string> firstNodeOfContext,
				int contextSearchId, ref IExpressionEvaluatorContext resultEec) {

			var eec = resultEec;
			// current checked node, can be null
			var currNode = firstNodeAfterSymbol;

			// for each node in context array (from left to right)
			for (var ctxtNode = firstNodeOfContext; ctxtNode != null; ctxtNode = ctxtNode.Next) {
				if (ctxtNode.IsSymbolNode) {

					var node = currNode;
					// skip all branches (non-symbol nodes)
					// A > B must match in "A B" and also in "A [x] [y] B"
					while (node != null && !node.IsSymbolNode) {
						node = node.Next;
					}

					// no symbol node found or its name do not match context
					if (node == null || node.Symbol.Name != ctxtNode.Symbol.Name) {
						return false;
					}

					eec = mapPatternConsts(ctxtNode.Symbol, node.Symbol, eec);
					currNode = node.Next;  // move current node
				}
				else {
					// try to match context on all following branches
					ContextListNode<IValue> matchedNode = null;
					for (var node = currNode; node != null && node.IsListNode; node = node.Next) {
						if (CheckRightContextOfSymbol(node.InnerList.First, ctxtNode.InnerList.First, contextSearchId, ref eec)) {
							if (node.MatchedId == contextSearchId) {
								continue;  // this node was already matched
							}

							node.MatchedId = contextSearchId;  // mark node as matched
							matchedNode = node;
							break;
						}
					}

					if (matchedNode == null) {
						return false;  // nothing matched
					}
					// do not move current node
				}
			}

			resultEec = eec;  // return EEC only if context is matched successfully
			return true;
		}


		private IExpressionEvaluatorContext mapPatternConsts(Symbol<string> pattern, Symbol<IValue> symbol, IExpressionEvaluatorContext eec) {

			int paramsLen = symbol.Arguments.Length;
			int patternLen = pattern.Arguments.Length;

			for (int i = 0; i < patternLen; i++) {
				// set value to NaN if symbol has not enough actual parameters to match pattern
				var value = i < paramsLen ? symbol.Arguments[i] : Constant.NaN;
				eec = eec.AddVariable(pattern.Arguments[i], value);
			}

			return eec;

		}

	}
}

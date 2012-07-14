/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Collections.Generic;
using System.Linq;
using Malsys.Evaluators;
using Malsys.Processing.Context;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Symbol = Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>;


namespace Malsys.Processing.Components.Rewriters {
	/// <summary>
	/// Full featured symbol rewriter which rewrites symbols based on defined rewrite rules in the L-system.
	/// It is capable to rewrite symbol based all criteria of Malsys' rewrite rules.
	/// Rewriting is initiated by symbol request (by enumerator).
	/// Then rewriter takes as many symbols from connected symbol provider as is needed for rewriting the symbol.
	/// If contexts (or branches) are long it may load many symbols before returning single one.
	/// </summary>
	/// <name>Symbol rewriter</name>
	/// <group>Rewriters</group>
	/// <remarks>
	/// Context is not freed yet.
	///
	/// The problem occurs when trying to check context of symbol which is ignored from context.
	/// </remarks>
	public partial class SymbolRewriter : IRewriter {

		private LsystemEvaled lsystem;
		private ProcessContext context;
		private IExpressionEvaluatorContext exprEvalCtxt;

		private SymbolRewriterEnumerator enumerator;

		private Dictionary<string, RewriterRewriteRule[]> rewriteRules;
		private HashSet<string> contextIgnoredSymbolNames = new HashSet<string>();
		private HashSet<string> startBranchSymbolNames = new HashSet<string>();
		private HashSet<string> endBranchSymbolNames = new HashSet<string>();
		private HashSet<string> contextSymbols = new HashSet<string>();

		private int leftCtxtMaxLen;
		private int rightCtxtMaxLen;
		private int rrReplacementMaxLen;
		private bool stochasticRules;


		public IMessageLogger Logger { get; set; }

		#region User gettable & settable properties

		/// <summary>
		/// List of symbols which are ignored in context checking.
		/// </summary>
		[AccessName("contextIgnore")]
		[UserSettableSybols]
		public ImmutableList<Symbol<IValue>> ContextIgnore {
			set {
				contextIgnoredSymbolNames.Clear();
				foreach (var sym in value) {
					contextIgnoredSymbolNames.Add(sym.Name);
				}
			}
		}

		/// <summary>
		/// List of symbols which are indicating start of branch.
		/// This symbols should be identical to symbols which are interpreted as start branch.
		/// </summary>
		[AccessName("startBranchSymbols")]
		[UserSettableSybols]
		public ImmutableList<Symbol<IValue>> StartBranchSymbols {
			set {
				startBranchSymbolNames.Clear();
				foreach (var sym in value) {
					startBranchSymbolNames.Add(sym.Name);
				}
			}
		}

		/// <summary>
		/// List of symbols which are indicating end of branch.
		/// This symbols should be identical to symbols which are interpreted as end branch.
		/// </summary>
		[AccessName("endBranchSymbols")]
		[UserSettableSybols]
		public ImmutableList<Symbol<IValue>> EndBranchSymbols {
			set {
				endBranchSymbolNames.Clear();
				foreach (var sym in value) {
					if (!endBranchSymbolNames.Contains(sym.Name)) {
						endBranchSymbolNames.Add(sym.Name);
					}
				}
			}
		}

		#endregion


		[UserConnectable]
		public ISymbolProvider SymbolProvider { get; set; }


		#region IComponent Members

		public void Reset() {
			ContextIgnore = StartBranchSymbols = EndBranchSymbols = ImmutableList<Symbol<IValue>>.Empty;
		}

		public void Initialize(ProcessContext ctxt) {

			context = ctxt;
			lsystem = ctxt.Lsystem;
			exprEvalCtxt = ctxt.ExpressionEvaluatorContext;

			contextSymbols.AddRange(contextIgnoredSymbolNames);
			contextSymbols.AddRange(startBranchSymbolNames);
			contextSymbols.AddRange(endBranchSymbolNames);

			rewriteRules = lsystem.RewriteRules
				.Select(x => new RewriterRewriteRule(
					x.LeftContext
						.Where(s => !contextIgnoredSymbolNames.Contains(s.Name))
						.Aggregate(
							new ContextListBuilder<string>(s => startBranchSymbolNames.Contains(s.Name), s => endBranchSymbolNames.Contains(s.Name)),
							(b, s) => { b.AddSymbolToContext(s); return b; })
						.RootNode.InnerList,
					x.RightContext
						.Where(s => !contextIgnoredSymbolNames.Contains(s.Name))
						.Aggregate(
							new ContextListBuilder<string>(s => startBranchSymbolNames.Contains(s.Name), s => endBranchSymbolNames.Contains(s.Name)),
							(b, s) => { b.AddSymbolToContext(s); return b; })
						.RootNode.InnerList) {
							SymbolPattern = x.SymbolPattern,
							LocalConstantDefs = x.LocalConstantDefs,
							Condition = x.Condition,
							Replacements = x.Replacements

						})
				.GroupBy(rr => rr.SymbolPattern.Name)
				.ToDictionary(g => g.Key, g => g.ToArray());

			leftCtxtMaxLen = lsystem.RewriteRules.Select(rr => rr.LeftContext.Length).DefaultIfEmpty().Max();
			rightCtxtMaxLen = lsystem.RewriteRules.Select(rr => rr.RightContext.Length).DefaultIfEmpty().Max();
			rrReplacementMaxLen = lsystem.RewriteRules
				.Select(rr => rr.Replacements.Select(replac => replac.Replacement.Length).DefaultIfEmpty().Max())
				.DefaultIfEmpty().Max();

			stochasticRules = lsystem.RewriteRules.Any(rr => rr.Replacements.Length > 1);

		}

		public void Cleanup() {
			context = null;
			lsystem = null;
			exprEvalCtxt = null;
			rewriteRules = null;
		}

		public void Dispose() { }



		public bool RequiresMeasure { get { return false; } }


		public void BeginProcessing(bool measuring) {
			enumerator = new SymbolRewriterEnumerator(this);
			SymbolProvider.BeginProcessing(measuring);
		}

		public void EndProcessing() {
			enumerator = null;
			SymbolProvider.EndProcessing();
		}

		#endregion


		/// <summary>
		/// Returns enumerator which will yields result of rewriting symbols from symbol provider.
		/// </summary>
		/// <remarks>
		/// Enumerator is reusable BUT Reset method must be called before each usage.
		/// Even source (symbol provider) can be switched between usages, Reset call will gets new enumerator.
		/// </remarks>
		public IEnumerator<Symbol> GetEnumerator() {
			enumerator.Reset();
			return enumerator;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			enumerator.Reset();
			return enumerator;
		}


		public enum Message {

			[Message(MessageType.Warning, "Function `{0}` does not exist or do not return a value. Emergency local random generator is used.")]
			NoRandomFunc,

		}


	}
}

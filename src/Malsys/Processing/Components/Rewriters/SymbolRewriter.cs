using System.Collections.Generic;
using System.Linq;
using Malsys.Evaluators;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Symbol = Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>;
using Malsys.Processing.Context;


namespace Malsys.Processing.Components.Rewriters {
	[Component("Symbol rewriter", ComponentGroupNames.Rewriters)]
	public partial class SymbolRewriter : IRewriter {


		private LsystemEvaled lsystem;
		private ProcessContext context;
		private IExpressionEvaluatorContext exprEvalCtxt;

		private SymbolRewriterEnumerator enumerator;

		private Dictionary<string, RewriterRewriteRule[]> rewriteRules;
		private HashSet<string> contextIgnoredSymbolNames;
		private HashSet<string> startBranchSymbolNames;
		private HashSet<string> endBranchSymbolNames;
		private HashSet<string> contextSymbols;

		private int leftCtxtMaxLen;
		private int rightCtxtMaxLen;
		private int rrReplacementMaxLen;
		private bool stochasticRules;


		[Alias("contextIgnore")]
		[UserSettableSybols]
		public ImmutableList<Symbol<IValue>> ContextIgnore {
			set {
				contextIgnoredSymbolNames = new HashSet<string>();
				foreach (var sym in value) {
					contextIgnoredSymbolNames.Add(sym.Name);
				}
			}
		}

		[Alias("startBranchSymbols")]
		[UserSettableSybols]
		public ImmutableList<Symbol<IValue>> StartBranchSymbols {
			set {
				startBranchSymbolNames = new HashSet<string>();
				foreach (var sym in value) {
					startBranchSymbolNames.Add(sym.Name);
				}
			}
		}

		[Alias("endBranchSymbols")]
		[UserSettableSybols]
		public ImmutableList<Symbol<IValue>> EndBranchSymbols {
			set {
				endBranchSymbolNames = new HashSet<string>();
				foreach (var sym in value) {
					if (!endBranchSymbolNames.Contains(sym.Name)) {
						endBranchSymbolNames.Add(sym.Name);
					}
				}
			}
		}


		#region IRewriter Members

		[UserConnectable]
		public ISymbolProvider SymbolProvider { get; set; }

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

		public bool RequiresMeasure {
			get { return false; }
		}

		public void Initialize(ProcessContext ctxt) {

			context = ctxt;
			lsystem = ctxt.Lsystem;
			exprEvalCtxt = ctxt.ExpressionEvaluatorContext;


			if (contextIgnoredSymbolNames == null) {
				contextIgnoredSymbolNames = new HashSet<string>();
			}

			if (startBranchSymbolNames == null) {
				startBranchSymbolNames = new HashSet<string>();
			}

			if (endBranchSymbolNames == null) {
				endBranchSymbolNames = new HashSet<string>();
			}

			contextSymbols = new HashSet<string>();
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

		}

		public void BeginProcessing(bool measuring) {

			enumerator = new SymbolRewriterEnumerator(this);

			SymbolProvider.BeginProcessing(measuring);
		}

		public void EndProcessing() {

			enumerator = null;

			SymbolProvider.EndProcessing();
		}

		#endregion


		public enum Message {

			[Message(MessageType.Warning, "Function `{0}` does not exist or do not return a value. Emergency local random generator is used.")]
			NoRandomFunc,

		}


	}
}

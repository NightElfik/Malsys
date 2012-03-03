using System.Collections.Generic;
using System.Linq;
using Malsys.Evaluators;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Symbol = Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>;


namespace Malsys.Processing.Components.Rewriters {
	[Component("Symbol rewriter", ComponentGroupNames.Rewriters)]
	public partial class SymbolRewriter : IRewriter {


		private LsystemEvaled lsystem;
		private IExpressionEvaluator exprEvaluator;

		private SymbolRewriterEnumerator enumerator;

		private Dictionary<string, RewriteRule[]> rewriteRules;
		private HashSet<string> contextIgnoredSymbolNames;

		private int leftCtxtMaxLen;
		private int rightCtxtMaxLen;
		private int rrReplacementMaxLen;
		private bool stochasticRules;


		[UserSettableSybols]
		public ImmutableList<Symbol<IValue>> ContextIgnore {
			set {
				contextIgnoredSymbolNames = new HashSet<string>();
				foreach (var sym in value) {
					if (!contextIgnoredSymbolNames.Contains(sym.Name)) {
						contextIgnoredSymbolNames.Add(sym.Name);
					}
				}
			}
		}


		[UserConnectable(IsOptional=true)]
		public IRandomGeneratorProvider RandomGeneratorProvider { get; set; }


		#region IRewriter Members

		[UserConnectable]
		public ISymbolProvider SymbolProvider { get; set; }

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

			lsystem = ctxt.Lsystem;
			exprEvaluator = ctxt.Evaluator.ResolveExpressionEvaluator();

			rewriteRules = lsystem.RewriteRules
				.GroupBy(rr => rr.SymbolPattern.Name)
				.ToDictionary(g => g.Key, g => g.ToArray());

			leftCtxtMaxLen = lsystem.RewriteRules.Select(rr => rr.LeftContext.Length).DefaultIfEmpty().Max();
			rightCtxtMaxLen = lsystem.RewriteRules.Select(rr => rr.RightContext.Length).DefaultIfEmpty().Max();
			rrReplacementMaxLen = lsystem.RewriteRules
				.Select(rr => rr.Replacements.Select(replac => replac.Replacement.Length).DefaultIfEmpty().Max())
				.DefaultIfEmpty().Max();

			stochasticRules = lsystem.RewriteRules.Any(rr => rr.Replacements.Length > 1);

			if (RandomGeneratorProvider == null) {
				ctxt.Logger.LogMessage(Message.NoRandomGenerator);
			}

			if (contextIgnoredSymbolNames == null) {
				// no symbols ignored
				contextIgnoredSymbolNames = new HashSet<string>();
			}

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

			[Message(MessageType.Warning, "No random generator was set. Local instance will be created.")]
			NoRandomGenerator,

		}


	}
}

using Malsys.SemanticModel;

namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class RewriteRuleReplacement {

		public static readonly RewriteRuleReplacement Empty = new RewriteRuleReplacement(SymbolsListExpr.Empty, Constant.One);


		public readonly SymbolsListExpr Replacement;
		public readonly IExpression Weight;

		public RewriteRuleReplacement(SymbolsListExpr replac, IExpression wei) {
			Replacement = replac;
			Weight = wei;
		}
	}
}

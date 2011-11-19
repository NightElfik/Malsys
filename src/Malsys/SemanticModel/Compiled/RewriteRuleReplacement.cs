
namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class RewriteRuleReplacement {

		public static readonly RewriteRuleReplacement Empty
			= new RewriteRuleReplacement(ImmutableList<Symbol<IExpression>>.Empty, Constant.One);


		public readonly ImmutableList<Symbol<IExpression>> Replacement;
		public readonly IExpression Weight;

		public RewriteRuleReplacement(ImmutableList<Symbol<IExpression>> replac, IExpression wei) {
			Replacement = replac;
			Weight = wei;
		}
	}
}

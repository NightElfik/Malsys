using System.Collections.Generic;

namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class SymbolsListExpr : ImmutableList<Symbol<IExpression>>, IBindable {

		new public static readonly SymbolsListExpr Empty = new SymbolsListExpr(ImmutableList<Symbol<IExpression>>.Empty);


		public SymbolsListExpr(IEnumerable<Symbol<IExpression>> symbols)
			: base(symbols) { }

		public SymbolsListExpr(ImmutableList<Symbol<IExpression>> symbols)
			: base(symbols) { }

	}
}

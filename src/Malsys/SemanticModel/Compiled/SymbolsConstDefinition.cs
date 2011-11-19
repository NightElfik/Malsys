
namespace Malsys.SemanticModel.Compiled {
	public class SymbolsConstDefinition : ILsystemStatement {

		public readonly string Name;
		public readonly ImmutableList<Symbol<IExpression>> Symbols;


		public SymbolsConstDefinition(string name, ImmutableList<Symbol<IExpression>> symbols) {

			Name = name;
			Symbols = symbols;
		}


		#region ILsystemStatement Members

		public LsystemStatementType StatementType {
			get { return LsystemStatementType.SymbolsConstant; }
		}

		#endregion
	}
}

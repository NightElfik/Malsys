
namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class SymbolsInterpretation : ILsystemStatement {

		public readonly ImmutableList<Symbol<VoidStruct>> Symbols;
		public readonly ImmutableList<OptionalParameter> Parameters;
		public readonly string InstructionName;
		public readonly ImmutableList<IExpression> InstructionParameters;

		public readonly Ast.SymbolsInterpretDef AstNode;


		public SymbolsInterpretation(ImmutableList<Symbol<VoidStruct>> symbols, ImmutableList<OptionalParameter> parameters,
				string instrName, ImmutableList<IExpression> defParams, Ast.SymbolsInterpretDef astNode) {

			Symbols = symbols;
			Parameters = parameters;
			InstructionName = instrName;
			InstructionParameters = defParams;

			AstNode = astNode;
		}


		public LsystemStatementType StatementType {
			get { return LsystemStatementType.SymbolsInterpretation; }
		}

	}
}

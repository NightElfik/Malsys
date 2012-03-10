
namespace Malsys.SemanticModel.Compiled {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class SymbolsInterpretation : ILsystemStatement {

		public readonly ImmutableList<Symbol<VoidStruct>> Symbols;
		public readonly ImmutableList<OptionalParameter> Parameters;
		public readonly string InstructionName;
		public readonly ImmutableList<IExpression> InstructionParameters;

		public readonly bool InstructionIsLsystemName;
		public readonly string LsystemConfigName;

		public readonly Ast.SymbolsInterpretDef AstNode;


		public SymbolsInterpretation(ImmutableList<Symbol<VoidStruct>> symbols, ImmutableList<OptionalParameter> parameters,
				string instrName, ImmutableList<IExpression> defParams, bool instructionIsLsystemName,
				string lsystemConfigName, Ast.SymbolsInterpretDef astNode) {

			Symbols = symbols;
			Parameters = parameters;
			InstructionName = instrName;
			InstructionParameters = defParams;
			InstructionIsLsystemName = instructionIsLsystemName;
			LsystemConfigName = lsystemConfigName;

			AstNode = astNode;
		}


		public LsystemStatementType StatementType {
			get { return LsystemStatementType.SymbolsInterpretation; }
		}

	}
}

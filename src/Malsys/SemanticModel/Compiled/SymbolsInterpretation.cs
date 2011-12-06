
namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class SymbolsInterpretation : ILsystemStatement {

		public readonly string InstructionName;
		public readonly ImmutableList<IExpression> DefaultParameters;
		public readonly ImmutableList<Symbol<VoidStruct>> Symbols;


		public SymbolsInterpretation(string instrName, ImmutableList<IExpression> defParams,
				ImmutableList<Symbol<VoidStruct>> symbols) {

			InstructionName = instrName;
			DefaultParameters = defParams;
			Symbols = symbols;
		}


		public LsystemStatementType StatementType {
			get { return LsystemStatementType.SymbolsInterpretation; }
		}

	}
}

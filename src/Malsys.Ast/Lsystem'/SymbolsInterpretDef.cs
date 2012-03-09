
namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class SymbolsInterpretDef : ILsystemStatement {

		public readonly ImmutableListPos<Identificator> Symbols;
		public readonly ImmutableListPos<OptionalParameter> Parameters;
		public readonly Identificator Instruction;
		public readonly ImmutableListPos<Expression> InstructionParameters;


		public SymbolsInterpretDef(ImmutableListPos<Identificator> symbols, ImmutableListPos<OptionalParameter> prms,
				Identificator instr, ImmutableListPos<Expression> instrParams , Position pos) {

			Symbols = symbols;
			Parameters = prms;
			Instruction = instr;
			InstructionParameters = instrParams;
		}


		public Position Position { get; private set; }


		public LsystemStatementType StatementType {
			get { return LsystemStatementType.SymbolsInterpretDef; }
		}

	}
}


namespace Malsys.Ast {
	public class SymbolsInterpretDef : ILsystemStatement {

		public ListPos<Identifier> Symbols;
		public ListPos<OptionalParameter> Parameters;
		public Identifier Instruction;
		public ListPos<Expression> InstructionParameters;

		public bool InstructionIsLsystemName;
		public Identifier LsystemConfigName;

		public PositionRange Position { get; private set; }


		public SymbolsInterpretDef(ListPos<Identifier> symbols, ListPos<OptionalParameter> prms,
				Identifier instr, ListPos<Expression> instrParams, PositionRange pos) {

			Symbols = symbols;
			Parameters = prms;
			Instruction = instr;
			InstructionParameters = instrParams;
			InstructionIsLsystemName = false;
			LsystemConfigName = new Identifier("", PositionRange.Unknown);
		}

		public SymbolsInterpretDef(ListPos<Identifier> symbols, ListPos<OptionalParameter> prms,
				Identifier instr, ListPos<Expression> instrParams, bool instructionIsLsystemName,
				Identifier lsystemConfigName, PositionRange pos) {

			Symbols = symbols;
			Parameters = prms;
			Instruction = instr;
			InstructionParameters = instrParams;
			InstructionIsLsystemName = instructionIsLsystemName;
			LsystemConfigName = lsystemConfigName;
		}




		public LsystemStatementType StatementType {
			get { return LsystemStatementType.SymbolsInterpretDef; }
		}

	}
}

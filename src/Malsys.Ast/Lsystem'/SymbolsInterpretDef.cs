/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class SymbolsInterpretDef : ILsystemStatement {

		public readonly ImmutableListPos<Identifier> Symbols;
		public readonly ImmutableListPos<OptionalParameter> Parameters;
		public readonly Identifier Instruction;
		public readonly ImmutableListPos<Expression> InstructionParameters;

		public readonly bool InstructionIsLsystemName;
		public readonly Identifier LsystemConfigName;


		public SymbolsInterpretDef(ImmutableListPos<Identifier> symbols, ImmutableListPos<OptionalParameter> prms,
				Identifier instr, ImmutableListPos<Expression> instrParams, PositionRange pos) {

			Symbols = symbols;
			Parameters = prms;
			Instruction = instr;
			InstructionParameters = instrParams;
			InstructionIsLsystemName = false;
			LsystemConfigName = Identifier.Empty;
		}

		public SymbolsInterpretDef(ImmutableListPos<Identifier> symbols, ImmutableListPos<OptionalParameter> prms,
				Identifier instr, ImmutableListPos<Expression> instrParams, bool instructionIsLsystemName,
				Identifier lsystemConfigName, PositionRange pos) {

			Symbols = symbols;
			Parameters = prms;
			Instruction = instr;
			InstructionParameters = instrParams;
			InstructionIsLsystemName = instructionIsLsystemName;
			LsystemConfigName = lsystemConfigName;
		}


		public PositionRange Position { get; private set; }


		public LsystemStatementType StatementType {
			get { return LsystemStatementType.SymbolsInterpretDef; }
		}

	}
}

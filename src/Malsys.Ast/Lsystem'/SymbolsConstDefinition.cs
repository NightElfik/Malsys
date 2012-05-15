/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class SymbolsConstDefinition : ILsystemStatement {

		public readonly Identifier NameId;
		public readonly ImmutableListPos<LsystemSymbol> SymbolsList;


		public SymbolsConstDefinition(Identifier name, ImmutableListPos<LsystemSymbol> symbols, PositionRange pos) {
			NameId = name;
			SymbolsList = symbols;
			Position = pos;
		}


		public PositionRange Position { get; private set; }


		public LsystemStatementType StatementType {
			get { return LsystemStatementType.SymbolsConstDefinition; }
		}

	}
}

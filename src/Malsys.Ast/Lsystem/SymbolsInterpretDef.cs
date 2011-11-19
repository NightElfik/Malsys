
namespace Malsys.Ast {
	public class SymbolsInterpretDef : ILsystemStatement {

		public readonly ImmutableListPos<LsystemSymbol> Symbols;
		public readonly Identificator Instruction;
		public readonly ImmutableListPos<Expression> DefaultParameters;


		public SymbolsInterpretDef(ImmutableListPos<LsystemSymbol> symbols, Identificator instr, ImmutableListPos<Expression> defParams , Position pos) {

			Symbols = symbols;
			Instruction = instr;
			DefaultParameters = defParams;
		}

		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstLsystemVisitable Members

		public void Accept(ILsystemVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

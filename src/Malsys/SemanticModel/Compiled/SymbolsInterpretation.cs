using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class SymbolsInterpretation : ILsystemStatement {

		public readonly string InstructionName;
		public readonly ImmutableList<IExpression> DefaultParameters;
		public readonly ImmutableList<Symbol<string>> Symbols;


		public SymbolsInterpretation(string instrName, ImmutableList<IExpression> defParams,
				ImmutableList<Symbol<string>> symbols) {

			InstructionName = instrName;
			DefaultParameters = defParams;
			Symbols = symbols;
		}


		#region ILsystemStatement Members

		public LsystemStatementType StatementType {
			get { return LsystemStatementType.SymbolsInterpretation; }
		}

		#endregion
	}
}

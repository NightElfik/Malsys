// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;

namespace Malsys.SemanticModel.Compiled {
	public class SymbolsInterpretation : ILsystemStatement {

		public List<Symbol<VoidStruct>> Symbols;
		public List<OptionalParameter> Parameters;
		public string InstructionName;
		public List<IExpression> InstructionParameters;

		public bool InstructionIsLsystemName;
		public string LsystemConfigName;

		public readonly Ast.SymbolsInterpretDef AstNode;


		public SymbolsInterpretation(Ast.SymbolsInterpretDef astNode) {
			AstNode = astNode;
		}


		public LsystemStatementType StatementType {
			get { return LsystemStatementType.SymbolsInterpretation; }
		}

	}
}

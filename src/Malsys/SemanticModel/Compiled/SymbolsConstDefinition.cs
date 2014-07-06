// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;

namespace Malsys.SemanticModel.Compiled {
	public class SymbolsConstDefinition : ILsystemStatement {

		public string Name;
		public List<Symbol<IExpression>> Symbols;

		public readonly Ast.SymbolsConstDefinition AstNode;


		public SymbolsConstDefinition(Ast.SymbolsConstDefinition astNode) {
			AstNode = astNode;
		}


		public LsystemStatementType StatementType {
			get { return LsystemStatementType.SymbolsConstant; }
		}

	}
}

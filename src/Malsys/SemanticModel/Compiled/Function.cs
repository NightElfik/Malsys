// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;

namespace Malsys.SemanticModel.Compiled {
	public class Function : IInputStatement, ILsystemStatement {

		public string Name;
		public List<OptionalParameter> Parameters;
		public List<IFunctionStatement> Statements;
		public Ast.FunctionDefinition AstNode;


		public Function(Ast.FunctionDefinition astNode) {
			AstNode = astNode;
		}


		InputStatementType IInputStatement.StatementType {
			get { return InputStatementType.Function; }
		}

		LsystemStatementType ILsystemStatement.StatementType {
			get { return LsystemStatementType.Function; }
		}

	}
}

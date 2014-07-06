// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;

namespace Malsys.SemanticModel.Compiled {
	public class Lsystem : IInputStatement {

		public string Name;
		public bool IsAbstract;
		public List<OptionalParameter> Parameters;
		public List<BaseLsystem> BaseLsystems;
		public List<ILsystemStatement> Statements;

		public readonly Ast.LsystemDefinition AstNode;


		public Lsystem(Ast.LsystemDefinition astNode) {
			AstNode = astNode;
		}


		public InputStatementType StatementType {
			get { return InputStatementType.Lsystem; }
		}

	}
}

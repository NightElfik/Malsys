// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;

namespace Malsys.SemanticModel.Compiled {
	public class ProcessConfigurationStatement : IInputStatement {

		public string Name;
		public List<ProcessComponent> Components;
		public List<ProcessContainer> Containers;
		public List<ProcessComponentsConnection> Connections;

		public readonly Ast.ProcessConfigurationDefinition AstNode;


		public ProcessConfigurationStatement(Ast.ProcessConfigurationDefinition astNode) {
			AstNode = astNode;
		}



		public InputStatementType StatementType {
			get { return InputStatementType.ProcessConfiguration; }
		}

	}
}

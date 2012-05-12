﻿/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.SemanticModel.Compiled {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessConfigurationStatement : IInputStatement {

		public readonly string Name;
		public readonly ImmutableList<ProcessComponent> Components;
		public readonly ImmutableList<ProcessContainer> Containers;
		public readonly ImmutableList<ProcessComponentsConnection> Connections;

		public readonly Ast.ProcessConfigurationDefinition AstNode;


		public ProcessConfigurationStatement(string name, ImmutableList<ProcessComponent> components, ImmutableList<ProcessContainer> containers,
				ImmutableList<ProcessComponentsConnection> connections, Ast.ProcessConfigurationDefinition astNode) {

			Name = name;
			Components = components;
			Containers = containers;
			Connections = connections;

			AstNode = astNode;
		}



		public InputStatementType StatementType {
			get { return InputStatementType.ProcessConfiguration; }
		}

	}
}

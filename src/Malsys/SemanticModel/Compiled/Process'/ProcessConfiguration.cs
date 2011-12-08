using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ProcessConfiguration : IInputStatement {

		public readonly string Name;
		public readonly ImmutableList<ProcessComponent> Components;
		public readonly ImmutableList<ProcessContainer> Containers;
		public readonly ImmutableList<ProcessComponentsConnection> Connections;


		public ProcessConfiguration(string name, ImmutableList<ProcessComponent> components, ImmutableList<ProcessContainer> containers,
				ImmutableList<ProcessComponentsConnection> connections) {

			Name = name;
			Components = components;
			Containers = containers;
			Connections = connections;

		}



		public InputStatementType StatementType {
			get { return InputStatementType.ProcessConfiguration; }
		}

	}
}

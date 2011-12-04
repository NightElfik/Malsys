using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ProcessConfiguration {

		public readonly ImmutableList<ProcessComponent> Components;
		public readonly ImmutableList<ProcessContainer> Containers;
		public readonly ImmutableList<ProcessComponentsConnection> Connections;


		public ProcessConfiguration(ImmutableList<ProcessComponent> components, ImmutableList<ProcessContainer> containers,
				ImmutableList<ProcessComponentsConnection> connections) {

			Components = components;
			Containers = containers;
			Connections = connections;

		}

	}
}

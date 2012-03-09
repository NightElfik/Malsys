﻿
namespace Malsys.SemanticModel.Compiled {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessComponentAssignment {

		public readonly string ComponentTypeName;
		public readonly string ContainerName;

		public ProcessComponentAssignment(string component, string container) {

			ComponentTypeName = component;
			ContainerName = container;
		}
	}
}

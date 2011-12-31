
namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ProcessConfiguration : IInputStatement {

		public readonly string Name;
		public readonly ImmutableList<ProcessComponent> Components;
		public readonly ImmutableList<ProcessContainer> Containers;
		public readonly ImmutableList<ProcessComponentsConnection> Connections;

		public readonly Ast.ProcessConfigurationDefinition AstNode;


		public ProcessConfiguration(string name, ImmutableList<ProcessComponent> components, ImmutableList<ProcessContainer> containers,
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

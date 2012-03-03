using System.Collections.Generic;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	internal class ProcessStatementsCompiler : IProcessStatementsCompiler, Ast.IProcessConfigVisitor {


		private IMessageLogger logger;
		private List<ProcessComponent> components;
		private List<ProcessContainer> containers;
		private List<ProcessComponentsConnection> connections;
		private HashSet<string> usedNames;


		public ProcessStatement Compile(Ast.ProcessStatement statement, IMessageLogger logger) {

			var assigns = statement.ComponentAssignments.Convert(a => new ProcessComponentAssignment(a.ComponentTypeNameId.Name, a.ContainerNameId.Name));

			return new ProcessStatement(statement.TargetLsystemNameId.Name, statement.ProcessConfiNameId.Name, assigns, statement);
		}


		public ProcessConfigurationStatement Compile(Ast.ProcessConfigurationDefinition processConfig, IMessageLogger logger) {

			this.logger = logger;
			components = new List<ProcessComponent>();
			containers = new List<ProcessContainer>();
			connections = new List<ProcessComponentsConnection>();
			usedNames = new HashSet<string>();

			foreach (var stat in processConfig.Statements) {
				stat.Accept(this);
			}

			var result = new ProcessConfigurationStatement(processConfig.NameId.Name, components.ToImmutableList(),
				containers.ToImmutableList(), connections.ToImmutableList(), processConfig);

			//  cleanup
			logger = null;
			components = null;
			containers = null;
			connections = null;
			usedNames = null;

			return result;
		}


		#region IProcessConfigVisitor Members

		public void Visit(Ast.EmptyStatement emptyStatement) {
			// do nothing
		}

		public void Visit(Ast.ProcessComponent component) {

			if (usedNames.Contains(component.NameId.Name)) {
				logger.LogMessage(Messages.ComponentNameNotUnique, component.NameId.Position, component.NameId.Name);
				return;
			}

			usedNames.Add(component.NameId.Name);
			components.Add(new ProcessComponent(component.NameId.Name, component.TypeNameId.Name));
		}

		public void Visit(Ast.ProcessContainer container) {

			if (usedNames.Contains(container.NameId.Name)) {
				logger.LogMessage(Messages.ContainerNameNotUnique, container.NameId.Position, container.NameId.Name);
				return;
			}

			usedNames.Add(container.NameId.Name);
			containers.Add(new ProcessContainer(container.NameId.Name, container.TypeNameId.Name, container.DefaultTypeNameId.Name));
		}

		public void Visit(Ast.ProcessConfigConnection connection) {

			if (!usedNames.Contains(connection.SourceNameId.Name)) {
				logger.LogMessage(Messages.ConnectionUnknownName, connection.SourceNameId.Position, connection.SourceNameId.Name);
				return;
			}

			if (!usedNames.Contains(connection.TargetNameId.Name)) {
				logger.LogMessage(Messages.ConnectionUnknownName, connection.TargetNameId.Position, connection.TargetNameId.Name);
				return;
			}

			connections.Add(new ProcessComponentsConnection(connection.SourceNameId.Name, connection.TargetNameId.Name, connection.TargetInputNameId.Name));
		}

		#endregion


		public enum Messages {

			[Message(MessageType.Error, "Component name `{0}` is not unique in process configuration.")]
			ComponentNameNotUnique,
			[Message(MessageType.Error, "Container name `{0}` is not unique in process configuration.")]
			ContainerNameNotUnique,
			[Message(MessageType.Error, "Unknown name `{0}` in connection.")]
			ConnectionUnknownName,

		}
	}
}

using System.Collections.Generic;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	/// <remarks>
	/// All public members are thread safe if supplied compilers are thread safe.
	/// </remarks>
	internal class ProcessStatementsCompiler : IProcessStatementsCompiler {

		private readonly IExpressionCompiler exprCompiler;


		public ProcessStatementsCompiler(IExpressionCompiler expressionCompiler) {
			exprCompiler = expressionCompiler;
		}


		public ProcessStatement Compile(Ast.ProcessStatement statement, IMessageLogger logger) {

			var assigns = statement.ComponentAssignments.Convert(a => new ProcessComponentAssignment(a.ComponentTypeNameId.Name, a.ContainerNameId.Name));
			var args = exprCompiler.CompileList(statement.Arguments, logger);

			return new ProcessStatement(statement.TargetLsystemNameId.Name, args, statement.ProcessConfiNameId.Name, assigns, statement);
		}


		public ProcessConfigurationStatement Compile(Ast.ProcessConfigurationDefinition processConfig, IMessageLogger logger) {

			var components = new List<ProcessComponent>();
			var containers = new List<ProcessContainer>();
			var connections = new List<ProcessComponentsConnection>();
			var usedNames = new HashSet<string>();

			foreach (var stat in processConfig.Statements) {

				switch (stat.StatementType) {

					case Ast.ProcessConfigStatementType.EmptyStatement:
						break;

					case Ast.ProcessConfigStatementType.ProcessComponent:
						var component = (Ast.ProcessComponent)stat;
						if (usedNames.Contains(component.NameId.Name)) {
							logger.LogMessage(Messages.ComponentNameNotUnique, component.NameId.Position, component.NameId.Name);
							break;
						}

						usedNames.Add(component.NameId.Name);
						components.Add(new ProcessComponent(component.NameId.Name, component.TypeNameId.Name));
						break;

					case Ast.ProcessConfigStatementType.ProcessContainer:
						var container = (Ast.ProcessContainer)stat;
						if (usedNames.Contains(container.NameId.Name)) {
							logger.LogMessage(Messages.ContainerNameNotUnique, container.NameId.Position, container.NameId.Name);
							break;
						}

						usedNames.Add(container.NameId.Name);
						containers.Add(new ProcessContainer(container.NameId.Name, container.TypeNameId.Name, container.DefaultTypeNameId.Name));
						break;

					case Ast.ProcessConfigStatementType.ProcessConfigConnection:
						var connection = (Ast.ProcessConfigConnection)stat;
						if (!connection.IsVirtual && !usedNames.Contains(connection.SourceNameId.Name)) {
							logger.LogMessage(Messages.ConnectionUnknownName, connection.SourceNameId.Position, connection.SourceNameId.Name);
							break;
						}

						if (!connection.IsVirtual && !usedNames.Contains(connection.TargetNameId.Name)) {
							logger.LogMessage(Messages.ConnectionUnknownName, connection.TargetNameId.Position, connection.TargetNameId.Name);
							break;
						}

						connections.Add(new ProcessComponentsConnection(connection.IsVirtual, connection.SourceNameId.Name,
							connection.TargetNameId.Name, connection.TargetInputNameId.Name));
						break;

					default:
						break;

				}

			}

			return new ProcessConfigurationStatement(processConfig.NameId.Name, components.ToImmutableList(),
				containers.ToImmutableList(), connections.ToImmutableList(), processConfig);

		}


		#region IProcessConfigVisitor Members

		public void Visit(Ast.EmptyStatement emptyStatement) {
			// do nothing
		}

		public void Visit(Ast.ProcessComponent component) {


		}

		public void Visit(Ast.ProcessContainer container) {


		}

		public void Visit(Ast.ProcessConfigConnection connection) {

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

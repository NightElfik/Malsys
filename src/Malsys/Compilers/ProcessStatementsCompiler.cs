using System.Collections.Generic;
// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Linq;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	/// <remarks>
	/// All public members are thread safe if supplied compilers are thread safe.
	/// </remarks>
	public class ProcessStatementsCompiler : IProcessStatementsCompiler {

		protected readonly IExpressionCompiler exprCompiler;
		protected readonly ILsystemCompiler lsysCompiler;


		public ProcessStatementsCompiler(ILsystemCompiler lsystemCompiler, IExpressionCompiler expressionCompiler) {
			lsysCompiler = lsystemCompiler;
			exprCompiler = expressionCompiler;
		}


		public ProcessStatement Compile(Ast.ProcessStatement statement, IMessageLogger logger) {
			return new ProcessStatement(statement) {
				TargetLsystemName = statement.TargetLsystemNameId.Name,
				Arguments = exprCompiler.CompileList(statement.Arguments, logger),
				ProcessConfiName = statement.ProcessConfiNameId.Name,
				ComponentAssignments = statement.ComponentAssignments.Select(a => new ProcessComponentAssignment(a) {
					ComponentTypeName = a.ComponentTypeNameId.Name,
					ContainerName = a.ContainerNameId.Name,
				}).ToList(),
				AdditionalLsystemStatements = lsysCompiler.CompileList(statement.AdditionalLsystemStatements, logger),
			};
		}


		public ProcessConfigurationStatement Compile(Ast.ProcessConfigurationDefinition processConfig, IMessageLogger logger) {

			var resultPcs = new ProcessConfigurationStatement(processConfig) {
				Name = processConfig.NameId.Name,
				Components = new List<ProcessComponent>(),
				Containers = new List<ProcessContainer>(),
				Connections = new List<ProcessComponentsConnection>(),
			};

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
						resultPcs.Components.Add(new ProcessComponent(component) {
							Name = component.NameId.Name,
							TypeName = component.TypeNameId.Name,
						});
						break;

					case Ast.ProcessConfigStatementType.ProcessContainer:
						var container = (Ast.ProcessContainer)stat;
						if (usedNames.Contains(container.NameId.Name)) {
							logger.LogMessage(Messages.ContainerNameNotUnique, container.NameId.Position, container.NameId.Name);
							break;
						}

						usedNames.Add(container.NameId.Name);
						resultPcs.Containers.Add(new ProcessContainer(container) {
							Name = container.NameId.Name,
							TypeName = container.TypeNameId.Name,
							DefaultTypeName = container.DefaultTypeNameId.Name,
						});
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

						resultPcs.Connections.Add(new ProcessComponentsConnection(connection) {
							IsVirtual = connection.IsVirtual,
							SourceName = connection.SourceNameId.Name,
							TargetName = connection.TargetNameId.Name,
							TargetInputName = connection.TargetInputNameId.Name,
						});
						break;

					default:
						break;

				}

			}

			return resultPcs;
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

		#endregion IProcessConfigVisitor Members


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

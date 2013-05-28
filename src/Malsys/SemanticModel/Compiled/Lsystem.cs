// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.SemanticModel.Compiled {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class Lsystem : IInputStatement {

		public readonly string Name;
		public readonly bool IsAbstract;
		public readonly ImmutableList<OptionalParameter> Parameters;
		public readonly ImmutableList<BaseLsystem> BaseLsystems;
		public readonly ImmutableList<ILsystemStatement> Statements;

		public readonly Ast.LsystemDefinition AstNode;


		public Lsystem(string name, bool isAbstract, ImmutableList<OptionalParameter> prms, ImmutableList<BaseLsystem> baseLsystems,
				ImmutableList<ILsystemStatement> statements, Ast.LsystemDefinition astNode) {

			Name = name;
			IsAbstract = isAbstract;
			Parameters = prms;
			BaseLsystems = baseLsystems;
			Statements = statements;

			AstNode = astNode;
		}


		public InputStatementType StatementType {
			get { return InputStatementType.Lsystem; }
		}

	}
}

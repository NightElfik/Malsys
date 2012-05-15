/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessConfigurationDefinition : IInputStatement {

		public readonly Identifier NameId;

		public readonly ImmutableListPos<IProcessConfigStatement> Statements;


		public ProcessConfigurationDefinition(Identifier name, ImmutableListPos<IProcessConfigStatement> statements, PositionRange pos) {

			NameId = name;
			Statements = statements;

			Position = pos;
		}


		public PositionRange Position { get; private set; }


		public InputStatementType StatementType {
			get { return InputStatementType.ProcessConfigurationDefinition; }
		}

	}
}

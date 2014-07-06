// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	public class ProcessConfigurationDefinition : IInputStatement {

		public Identifier NameId;

		public ListPos<IProcessConfigStatement> Statements;

		public PositionRange Position { get; private set; }


		public ProcessConfigurationDefinition(Identifier name, ListPos<IProcessConfigStatement> statements, PositionRange pos) {

			NameId = name;
			Statements = statements;

			Position = pos;
		}


		public InputStatementType StatementType {
			get { return InputStatementType.ProcessConfigurationDefinition; }
		}

	}
}

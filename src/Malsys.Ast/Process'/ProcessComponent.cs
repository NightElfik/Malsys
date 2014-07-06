// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	public class ProcessComponent : IProcessConfigStatement {

		public Identifier NameId;
		public Identifier TypeNameId;

		public PositionRange Position { get; private set; }


		public ProcessComponent(Identifier name, Identifier typeName, PositionRange pos) {

			NameId = name;
			TypeNameId = typeName;

			Position = pos;
		}


		public ProcessConfigStatementType StatementType {
			get { return ProcessConfigStatementType.ProcessComponent; }
		}

	}
}

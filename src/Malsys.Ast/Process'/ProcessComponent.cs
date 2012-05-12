/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessComponent : IProcessConfigStatement {

		public readonly Identificator NameId;
		public readonly Identificator TypeNameId;


		public ProcessComponent(Identificator name, Identificator typeName, Position pos) {

			NameId = name;
			TypeNameId = typeName;

			Position = pos;
		}


		public Position Position { get; private set; }


		public ProcessConfigStatementType StatementType {
			get { return ProcessConfigStatementType.ProcessComponent; }
		}

	}
}

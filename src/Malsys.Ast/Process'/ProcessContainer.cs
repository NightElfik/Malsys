/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessContainer : IProcessConfigStatement {

		public readonly Identifier NameId;
		public readonly Identifier TypeNameId;
		public readonly Identifier DefaultTypeNameId;


		public ProcessContainer(Identifier name, Identifier typeName, Identifier defaultTypeName, PositionRange pos) {

			NameId = name;
			TypeNameId = typeName;
			DefaultTypeNameId = defaultTypeName;

			Position = pos;
		}


		public PositionRange Position { get; private set; }


		public ProcessConfigStatementType StatementType {
			get { return ProcessConfigStatementType.ProcessContainer; }
		}

	}
}

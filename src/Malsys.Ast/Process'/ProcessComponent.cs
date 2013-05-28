// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessComponent : IProcessConfigStatement {

		public readonly Identifier NameId;
		public readonly Identifier TypeNameId;


		public ProcessComponent(Identifier name, Identifier typeName, PositionRange pos) {

			NameId = name;
			TypeNameId = typeName;

			Position = pos;
		}


		public PositionRange Position { get; private set; }


		public ProcessConfigStatementType StatementType {
			get { return ProcessConfigStatementType.ProcessComponent; }
		}

	}
}

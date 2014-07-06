// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessConfigConnection : IProcessConfigStatement {

		public bool IsVirtual;
		public Identifier SourceNameId;
		public Identifier TargetNameId;
		public Identifier TargetInputNameId;

		public PositionRange Position { get; private set; }


		public ProcessConfigConnection(bool isVirtual, Identifier sourceName, Identifier targetName,
				Identifier targetInputName, PositionRange pos) {

			IsVirtual = isVirtual;
			SourceNameId = sourceName;
			TargetNameId = targetName;
			TargetInputNameId = targetInputName;

			Position = pos;
		}


		public ProcessConfigStatementType StatementType {
			get { return ProcessConfigStatementType.ProcessConfigConnection; }
		}

	}
}

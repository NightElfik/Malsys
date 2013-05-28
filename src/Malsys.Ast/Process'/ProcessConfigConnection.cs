// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessConfigConnection : IProcessConfigStatement {

		public readonly bool IsVirtual;
		public readonly Identifier SourceNameId;
		public readonly Identifier TargetNameId;
		public readonly Identifier TargetInputNameId;


		public ProcessConfigConnection(bool isVirtual, Identifier sourceName, Identifier targetName, Identifier targetInputName, PositionRange pos) {

			IsVirtual = isVirtual;
			SourceNameId = sourceName;
			TargetNameId = targetName;
			TargetInputNameId = targetInputName;

			Position = pos;
		}


		public PositionRange Position { get; private set; }


		public ProcessConfigStatementType StatementType {
			get { return ProcessConfigStatementType.ProcessConfigConnection; }
		}

	}
}

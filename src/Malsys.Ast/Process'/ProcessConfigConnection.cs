/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessConfigConnection : IProcessConfigStatement {

		public readonly bool IsVirtual;
		public readonly Identificator SourceNameId;
		public readonly Identificator TargetNameId;
		public readonly Identificator TargetInputNameId;


		public ProcessConfigConnection(bool isVirtual, Identificator sourceName, Identificator targetName, Identificator targetInputName, Position pos) {

			IsVirtual = isVirtual;
			SourceNameId = sourceName;
			TargetNameId = targetName;
			TargetInputNameId = targetInputName;

			Position = pos;
		}


		public Position Position { get; private set; }


		public ProcessConfigStatementType StatementType {
			get { return ProcessConfigStatementType.ProcessConfigConnection; }
		}

	}
}

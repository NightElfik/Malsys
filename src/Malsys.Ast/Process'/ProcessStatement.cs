// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessStatement : IInputStatement {

		public Identifier TargetLsystemNameId;
		public ListPos<Expression> Arguments;
		public Identifier ProcessConfiNameId;
		public ListPos<ProcessComponentAssignment> ComponentAssignments;
		public ListPos<ILsystemStatement> AdditionalLsystemStatements;


		public PositionRange Position { get; private set; }


		public ProcessStatement(Identifier targetLsystemName, ListPos<Expression> arguments,
				Identifier processConfiNameId, ListPos<ProcessComponentAssignment> componentAssignments,
				ListPos<ILsystemStatement> additionalLsystemStatements, PositionRange pos) {

			TargetLsystemNameId = targetLsystemName;
			Arguments = arguments;
			ProcessConfiNameId = processConfiNameId;
			ComponentAssignments = componentAssignments;
			AdditionalLsystemStatements = additionalLsystemStatements;

			Position = pos;
		}


		public InputStatementType StatementType {
			get { return InputStatementType.ProcessStatement; }
		}

	}
}

/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessStatement : IInputStatement {

		public readonly Identifier TargetLsystemNameId;

		public readonly ImmutableListPos<Expression> Arguments;

		public readonly Identifier ProcessConfiNameId;

		public readonly ImmutableListPos<ProcessComponentAssignment> ComponentAssignments;

		public readonly ImmutableListPos<ILsystemStatement> AdditionalLsystemStatements;


		public ProcessStatement(Identifier targetLsystemName, ImmutableListPos<Expression> arguments, Identifier processConfiNameId,
				ImmutableListPos<ProcessComponentAssignment> componentAssignments, ImmutableListPos<ILsystemStatement> additionalLsystemStatements, PositionRange pos) {

			TargetLsystemNameId = targetLsystemName;
			Arguments = arguments;
			ProcessConfiNameId = processConfiNameId;
			ComponentAssignments = componentAssignments;
			AdditionalLsystemStatements = additionalLsystemStatements;

			Position = pos;
		}


		public PositionRange Position { get; private set; }

		public InputStatementType StatementType {
			get { return InputStatementType.ProcessStatement; }
		}

	}
}

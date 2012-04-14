
namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessStatement : IInputStatement {

		public readonly Identificator TargetLsystemNameId;

		public readonly ImmutableListPos<Expression> Arguments;

		public readonly Identificator ProcessConfiNameId;

		public readonly ImmutableListPos<ProcessComponentAssignment> ComponentAssignments;

		public readonly ImmutableListPos<ILsystemStatement> AdditionalLsystemStatements;


		public ProcessStatement(Identificator targetLsystemName, ImmutableListPos<Expression> arguments, Identificator processConfiNameId,
				ImmutableListPos<ProcessComponentAssignment> componentAssignments, ImmutableListPos<ILsystemStatement> additionalLsystemStatements, Position pos) {

			TargetLsystemNameId = targetLsystemName;
			Arguments = arguments;
			ProcessConfiNameId = processConfiNameId;
			ComponentAssignments = componentAssignments;
			AdditionalLsystemStatements = additionalLsystemStatements;

			Position = pos;
		}


		public Position Position { get; private set; }

		public InputStatementType StatementType {
			get { return InputStatementType.ProcessStatement; }
		}

	}
}

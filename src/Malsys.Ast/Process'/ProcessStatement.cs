
namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessStatement : IInputStatement {

		public readonly Identificator TargetLsystemNameId;

		public readonly ImmutableListPos<Expression> Arguments;

		public readonly Identificator ProcessConfiNameId;

		public readonly ImmutableListPos<ProcessComponentAssignment> ComponentAssignments;


		public ProcessStatement(Identificator targetLsystemName, ImmutableListPos<Expression> arguments, Identificator processConfiNameId,
				ImmutableListPos<ProcessComponentAssignment> componentAssignments, Position pos) {

			TargetLsystemNameId = targetLsystemName;
			Arguments = arguments;
			ProcessConfiNameId = processConfiNameId;
			ComponentAssignments = componentAssignments;

			Position = pos;
		}


		public Position Position { get; private set; }

		public InputStatementType StatementType {
			get { return InputStatementType.ProcessStatement; }
		}

	}
}

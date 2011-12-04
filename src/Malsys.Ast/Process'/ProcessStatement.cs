
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ProcessStatement : IInputStatement, ILsystemStatement {

		public readonly Identificator TargetLsystemNameId;

		public readonly Identificator ProcessConfiNameId;

		public readonly ImmutableListPos<ProcessComponentAssignment> ComponentAssignments;


		public ProcessStatement(Identificator targetLsystemName, Identificator processConfiNameId,
				ImmutableListPos<ProcessComponentAssignment> componentAssignments, Position pos) {

			TargetLsystemNameId = targetLsystemName;
			ProcessConfiNameId = processConfiNameId;
			ComponentAssignments = componentAssignments;

			Position = pos;
		}


		public Position Position { get; private set; }

		public void Accept(IInputVisitor visitor) {
			visitor.Visit(this);
		}

		public void Accept(ILsystemVisitor visitor) {
			visitor.Visit(this);
		}
	}
}

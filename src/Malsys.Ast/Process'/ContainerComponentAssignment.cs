
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ProcessComponentAssignment : IToken {

		public readonly Identificator ComponentNameId;
		public readonly Identificator ContainerNameId;


		public ProcessComponentAssignment(Identificator component, Identificator container, Position pos) {

			ComponentNameId = component;
			ContainerNameId = container;

			Position = pos;
		}


		public Position Position { get; private set; }

	}
}

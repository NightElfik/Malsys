
namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessConfigConnection : IProcessConfigStatement {

		public readonly Identificator SourceNameId;
		public readonly Identificator TargetNameId;
		public readonly Identificator TargetInputNameId;


		public ProcessConfigConnection(Identificator sourceName, Identificator targetName, Identificator targetInputName, Position pos) {

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

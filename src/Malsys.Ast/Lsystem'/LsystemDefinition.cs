
namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class LsystemDefinition : NameParamsStatements<ILsystemStatement>, IInputStatement {

		public LsystemDefinition(Identificator name, ImmutableListPos<OptionalParameter> prms,
			ImmutableListPos<ILsystemStatement> statements, Position pos) : base(name, prms, statements, pos) {		}


		public InputStatementType StatementType {
			get { return InputStatementType.LsystemDefinition; }
		}

	}
}


namespace Malsys.Ast {
	public class LsystemDefinition : NameParamsStatements<ILsystemStatement>, IInputStatement {

		public bool IsAbstract;

		public ListPos<BaseLsystem> BaseLsystems;


		public LsystemDefinition(Identifier name, bool isAbstract, ListPos<OptionalParameter> prms,
				ListPos<BaseLsystem> baseLsystems, ListPos<ILsystemStatement> statements, PositionRange pos)
			: base(name, prms, statements, pos) {

			IsAbstract = isAbstract;
			BaseLsystems = baseLsystems;
		}


		public InputStatementType StatementType {
			get { return InputStatementType.LsystemDefinition; }
		}

	}
}

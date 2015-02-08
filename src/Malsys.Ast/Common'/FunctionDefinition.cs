
namespace Malsys.Ast {
	public class FunctionDefinition : NameParamsStatements<IFunctionStatement>, IInputStatement, ILsystemStatement {

		public int ParametersCount { get { return Parameters.Count; } }


		public FunctionDefinition(Identifier name, ListPos<OptionalParameter> prms,
				ListPos<IFunctionStatement> statements, PositionRange pos)
			: base(name, prms, statements, pos) { }


		InputStatementType IInputStatement.StatementType {
			get { return InputStatementType.FunctionDefinition; }
		}

		LsystemStatementType ILsystemStatement.StatementType {
			get { return LsystemStatementType.FunctionDefinition; }
		}

	}
}

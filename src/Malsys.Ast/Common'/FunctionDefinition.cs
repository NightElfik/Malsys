
namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class FunctionDefinition : NameParamsStatements<IFunctionStatement>, IInputStatement, ILsystemStatement {

		public FunctionDefinition(Identificator name, ImmutableListPos<OptionalParameter> prms,
				ImmutableListPos<IFunctionStatement> statements, Position pos)
			: base(name, prms, statements, pos) { }


		public int ParametersCount { get { return Parameters.Length; } }


		InputStatementType IInputStatement.StatementType {
			get { return InputStatementType.FunctionDefinition; }
		}

		LsystemStatementType ILsystemStatement.StatementType {
			get { return LsystemStatementType.FunctionDefinition; }
		}

	}
}

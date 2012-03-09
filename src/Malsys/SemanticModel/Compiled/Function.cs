
namespace Malsys.SemanticModel.Compiled {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class Function : IInputStatement, ILsystemStatement {

		public readonly string Name;
		public readonly ImmutableList<OptionalParameter> Parameters;
		public readonly ImmutableList<IFunctionStatement> Statements;
		public readonly Ast.FunctionDefinition AstNode;


		public Function(string name, ImmutableList<OptionalParameter> prms, ImmutableList<IFunctionStatement> stats,
				Ast.FunctionDefinition astNode) {

			Name = name;
			Parameters = prms;
			Statements = stats;
			AstNode = astNode;
		}


		InputStatementType IInputStatement.StatementType {
			get { return InputStatementType.Function; }
		}


		LsystemStatementType ILsystemStatement.StatementType {
			get { return LsystemStatementType.Function; }
		}

	}
}


namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
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

		#region IInputStatement Members

		InputStatementType IInputStatement.StatementType {
			get { return InputStatementType.Function; }
		}

		#endregion

		#region ILsystemStatement Members

		LsystemStatementType ILsystemStatement.StatementType {
			get { return LsystemStatementType.Function; }
		}

		#endregion
	}
}

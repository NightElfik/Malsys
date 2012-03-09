
namespace Malsys.SemanticModel.Compiled {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ConstantDefinition : IInputStatement, ILsystemStatement, IFunctionStatement {

		public readonly string Name;

		public readonly IExpression Value;

		public readonly bool IsComponentAssign;

		public readonly Ast.ConstantDefinition AstNode;


		public ConstantDefinition(string name, IExpression value, bool isComponentAssign, Ast.ConstantDefinition astNode) {
			Name = name;
			Value = value;
			IsComponentAssign = isComponentAssign;
			AstNode = astNode;
		}


		InputStatementType IInputStatement.StatementType {
			get { return InputStatementType.Constant; }
		}

		LsystemStatementType ILsystemStatement.StatementType {
			get { return LsystemStatementType.Constant; }
		}

		FunctionStatementType IFunctionStatement.StatementType {
			get { return FunctionStatementType.ConstantDefinition; }
		}

	}
}

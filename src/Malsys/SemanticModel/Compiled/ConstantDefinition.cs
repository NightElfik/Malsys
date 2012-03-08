
namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
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


		// IInputStatement
		InputStatementType IInputStatement.StatementType {
			get { return InputStatementType.Constant; }
		}

		// ILsystemStatement
		LsystemStatementType ILsystemStatement.StatementType {
			get { return LsystemStatementType.Constant; }
		}

		// IFunctionStatement
		FunctionStatementType IFunctionStatement.StatementType {
			get { return FunctionStatementType.ConstantDefinition; }
		}

	}
}

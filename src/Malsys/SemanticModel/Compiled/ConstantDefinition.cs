
namespace Malsys.SemanticModel.Compiled {
	public class ConstantDefinition : IInputStatement, ILsystemStatement, IFunctionStatement {

		public string Name;
		public IExpression Value;
		public bool IsComponentAssign;

		public readonly Ast.ConstantDefinition AstNode;


		public ConstantDefinition(Ast.ConstantDefinition astNode) {
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

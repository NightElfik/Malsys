
namespace Malsys.Ast {
	public class ConstantDefinition : IInputStatement, ILsystemStatement, IFunctionStatement {

		public Identifier NameId;
		public Expression ValueExpr;
		public bool IsComponentAssign;

		public PositionRange Position { get; private set; }


		public ConstantDefinition(Identifier name, Expression value, PositionRange pos) {
			NameId = name;
			ValueExpr = value;
			IsComponentAssign = false;
			Position = pos;
		}

		public ConstantDefinition(Identifier name, Expression value, bool isComponentAssign, PositionRange pos) {
			NameId = name;
			ValueExpr = value;
			IsComponentAssign = isComponentAssign;
			Position = pos;
		}




		InputStatementType IInputStatement.StatementType {
			get { return InputStatementType.ConstantDefinition; }
		}


		LsystemStatementType ILsystemStatement.StatementType {
			get { return LsystemStatementType.ConstantDefinition; }
		}


		FunctionStatementType IFunctionStatement.StatementType {
			get { return FunctionStatementType.ConstantDefinition; }
		}

	}
}


namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ConstantDefinition : IInputStatement, ILsystemStatement, IFunctionStatement {

		public readonly Identificator NameId;

		public readonly Expression ValueExpr;

		public readonly bool IsComponentAssign;


		public ConstantDefinition(Identificator name, Expression value, Position pos) {
			NameId = name;
			ValueExpr = value;
			IsComponentAssign = false;
			Position = pos;
		}

		public ConstantDefinition(Identificator name, Expression value, bool isComponentAssign, Position pos) {
			NameId = name;
			ValueExpr = value;
			IsComponentAssign = isComponentAssign;
			Position = pos;
		}


		public Position Position { get; private set; }


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

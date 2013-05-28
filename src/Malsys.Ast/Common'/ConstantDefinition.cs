// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ConstantDefinition : IInputStatement, ILsystemStatement, IFunctionStatement {

		public readonly Identifier NameId;

		public readonly Expression ValueExpr;

		public readonly bool IsComponentAssign;


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


		public PositionRange Position { get; private set; }


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

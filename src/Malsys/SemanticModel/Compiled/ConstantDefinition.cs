using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ConstantDefinition : IInputStatement, ILsystemStatement, IFunctionStatement {

		public readonly string Name;
		public readonly IExpression Value;

		public readonly Ast.ConstantDefinition AstNode;


		public ConstantDefinition(string name, IExpression value, Ast.ConstantDefinition astNode) {
			Name = name;
			Value = value;
			AstNode = astNode;
		}

		#region IInputStatement Members

		InputStatementType IInputStatement.StatementType {
			get { return InputStatementType.Constant; }
		}

		#endregion

		#region ILsystemStatement Members

		LsystemStatementType ILsystemStatement.StatementType {
			get { return LsystemStatementType.Constant; }
		}

		#endregion

		#region IFunctionStatement Members

		FunctionStatementType IFunctionStatement.StatementType {
			get { return FunctionStatementType.ConstantDefinition; }
		}

		#endregion
	}
}

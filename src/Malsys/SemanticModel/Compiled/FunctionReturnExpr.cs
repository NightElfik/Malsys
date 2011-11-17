
namespace Malsys.SemanticModel.Compiled {
	class FunctionReturnExpr : IFunctionStatement {

		public readonly IExpression ReturnValue;


		public FunctionReturnExpr(IExpression retVal) {
			ReturnValue = retVal;
		}


		#region IFunctionStatement Members

		public FunctionStatementType StatementType {
			get { return FunctionStatementType.ReturnExpression; }
		}

		#endregion
	}
}

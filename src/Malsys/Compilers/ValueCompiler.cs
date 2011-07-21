using Malsys.Ast;
using Malsys.Expressions;

namespace Malsys.Compilers {
	public static class ValueCompiler {

		public static bool TryCompile(Ast.IValue value, ExpressionCompilerParameters prms, out IExpressionValue result) {
			if (value.IsExpression) {
				PostfixExpression pe;
				if (ExpressionCompiler.TryCompile((Expression)value, prms, out pe)) {
					result = pe;
					return true;
				}
				else {
					result = null;
					return false;
				}
			}
			else {
				var arr = (Ast.ValuesArray)value;
				IExpressionValue[] resArr = new IExpressionValue[arr.Length];

				for (int i = 0; i < resArr.Length; i++) {
					IExpressionValue val;
					if (TryCompile(arr[i], prms, out val)) {
						resArr[i] = val;
					}
				}

				if (prms.Messages.ErrorOcured) {
					result = null;
					return false;
				}

				result = new ExpressionValuesArray(resArr);
				return true;
			}
		}

	}
}

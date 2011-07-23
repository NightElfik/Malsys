using Malsys.Ast;
using Malsys.Expressions;

namespace Malsys.Compilers {
	public static class ValueCompiler {

		public static bool TryCompile(Ast.IValue value, ExpressionCompilerParameters prms, out IExpression result) {
			if (value.IsExpression) {
				IExpression expr;
				if (ExpressionCompiler.TryCompile((Expression)value, prms, out expr)) {
					result = expr;
					return true;
				}
			}
			else {
				ExpressionValuesArray rsltArr;
				if (TryCompile((Ast.ValuesArray)value, prms, out rsltArr)) {
					result = rsltArr;
					return true;
				}
			}

			result = null;
			return false;
		}

		public static bool TryCompile(Ast.ValuesArray arr, ExpressionCompilerParameters prms, out ExpressionValuesArray result) {
			IExpression[] resArr = new IExpression[arr.Length];

			for (int i = 0; i < resArr.Length; i++) {
				IExpression val;
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

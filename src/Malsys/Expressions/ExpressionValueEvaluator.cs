using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.UserFunction>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Expressions {
	public static class ExpressionValueEvaluator {

		public static IValue Evaluate(IExpressionValue value, VarMap variables, FunMap functions) {
			if (value.IsExpression) {
				return PostfixExpressionsEvaluator.Evaluate((PostfixExpression)value, variables, functions);
			}
			else {
				var arr = (ExpressionValuesArray)value;
				var resultArr = new IValue[arr.Length];

				for (int i = 0; i < resultArr.Length; i++) {
					resultArr[i] = Evaluate(arr[i], variables, functions);
				}

				return new ValuesArray(resultArr);
			}
		}
	}
}

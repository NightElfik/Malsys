using System;
using System.Diagnostics;
using Malsys.Expressions;
using System.Text;

namespace Malsys {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FunctionDefinition : RichExpression {

		public readonly string Name;
		public readonly int ParametersCount;
		public readonly int MandatoryParamsCount;
		public readonly int OptionalParamsCount;

		public readonly ImmutableList<string> ParametersNames;
		/// <summary>
		/// Aligned to the end of all parameters, last item from this array is last parameter's optional value expression.
		/// </summary>
		public readonly ImmutableList<IValue> OptionalParamsValues;


		public FunctionDefinition(string name, ImmutableList<string> parNames, ImmutableList<IValue> optParamsVals, ImmutableList<VariableDefinition> varDefs, IExpression expr)
			: base(varDefs, expr) {

			Name = name;
			ParametersNames = parNames;
			OptionalParamsValues = optParamsVals;

			ParametersCount = ParametersNames.Length;
			OptionalParamsCount = OptionalParamsValues.Length;
			MandatoryParamsCount = ParametersCount - OptionalParamsCount;

			Debug.Assert(MandatoryParamsCount >= 0, "Function can not have more default params than parameters count.");
		}

		public IValue GetOptionalParamValue(int i) {
			int actualI = i - MandatoryParamsCount;
			if (actualI < 0 || i >= ParametersCount) {
				throw new ArgumentException("Invalid optional parameter index {0} in user function `{1}`.".Fmt(i, Name));
			}

			return OptionalParamsValues[actualI];
		}
	}
}

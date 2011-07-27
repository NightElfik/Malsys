using System;
using System.Diagnostics;
using Malsys.Expressions;

namespace Malsys {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class LsystemDefinition {

		public readonly string Name;
		public readonly int ParametersCount;
		public readonly int MandatoryParamsCount;
		public readonly int OptionalParamsCount;

		public readonly ImmutableList<string> ParametersNames;
		/// <summary>
		/// Aligned to the end of all parameters, last item from this array is last parameter's optional value expression.
		/// </summary>
		public readonly ImmutableList<IValue> OptionalParamsValues;

		public readonly ImmutableList<FunctionDefinition> Functions;
		public readonly ImmutableList<VariableDefinition> Variables;

		public readonly ImmutableList<RewriteRule> RewriteRules;


		public LsystemDefinition(string name, ImmutableList<string> parNames, ImmutableList<IValue> optParamsVals,
				ImmutableList<FunctionDefinition> funs, ImmutableList<VariableDefinition> vars, ImmutableList<RewriteRule> rRules) {

			Name = name;
			ParametersNames = parNames;
			OptionalParamsValues = optParamsVals;
			Functions = funs;
			Variables = vars;
			RewriteRules = rRules;

			ParametersCount = ParametersNames.Length;
			OptionalParamsCount = OptionalParamsValues.Length;
			MandatoryParamsCount = ParametersCount - OptionalParamsCount;

			Debug.Assert(MandatoryParamsCount >= 0, "L-system can not have more default params than parameters count.");
		}

		public IValue GetOptionalParamValue(int i) {
			int actualI = i - MandatoryParamsCount;
			if (actualI < 0 || i >= ParametersCount) {
				throw new ArgumentException("Invalid optional parameter index {0} in L-system `{1}`.".Fmt(i, Name));
			}

			return OptionalParamsValues[actualI];
		}
	}
}

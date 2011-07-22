using System;
using System.Diagnostics;
using Malsys.Expressions;

namespace Malsys {
	public class FunctionDefinition {

		public readonly string Name;
		public readonly int ParametersCount;
		public readonly int MandatoryParamsCount;
		public readonly int OptionalParamsCount;
		public readonly int LocalVariableDefsCount;
		public readonly IExpression Expression;

		private string[] paramsNames;
		/// <summary>
		/// Aligned to the end, last item from this array is last parameter.
		/// </summary>
		private IExpression[] optionalParamsValues;
		private VariableDefinition[] variableDefs;


		public FunctionDefinition(string name, string[] parNames, IExpression[] optParamsVals, VariableDefinition[] varDefs, IExpression expr) {
			Name = name;
			paramsNames = parNames;
			optionalParamsValues = optParamsVals;
			variableDefs = varDefs;
			Expression = expr;

			ParametersCount = paramsNames.Length;
			OptionalParamsCount = optionalParamsValues.Length;
			MandatoryParamsCount = ParametersCount - OptionalParamsCount;
			LocalVariableDefsCount = variableDefs.Length;

			Debug.Assert(MandatoryParamsCount < 0, "Function can not have more default params than parameters count.");
		}


		public string GetParamName(int i) {
			return paramsNames[i];
		}

		public IExpression GetOptionalParamValue(int i) {
			int actualI = i - MandatoryParamsCount;
			if (actualI < 0 || i >= ParametersCount) {
				throw new ArgumentException("Invalid optional parameter index {0} in user function `{1}`.".Fmt(i, Name));
			}

			return optionalParamsValues[actualI];
		}

		public VariableDefinition GetVariableDefinition(int i) {
			return variableDefs[i];
		}
	}
}

using System;
using System.Collections.Generic;
using System.Reflection;
using Malsys.Evaluators;
using Malsys.Resources;

namespace Malsys.Reflection {
	public class FunctionDumper {

		public IEnumerable<FunctionInfo> DumpStaticFunctionDefinitions(Type t) {

			foreach (var fieldInfo in t.GetFields(BindingFlags.Public | BindingFlags.Static)) {

				if (fieldInfo.FieldType != typeof(FunctionCore)) {
					continue;
				}

				var value = (FunctionCore)fieldInfo.GetValue(null);

				foreach (var name in fieldInfo.GetAliases()) {
					yield return new FunctionInfo(name, value.ParametersCount, value.EvalFunction, value.ParamsTypesCyclicPattern, fieldInfo);
				}

			}

		}

		public IExpressionEvaluatorContext RegiterAllFunctions(Type t, IExpressionEvaluatorContext eec) {

			foreach (var fun in DumpStaticFunctionDefinitions(t)) {
				eec = eec.AddFunction(fun);
			}

			return eec;

		}

	}
}

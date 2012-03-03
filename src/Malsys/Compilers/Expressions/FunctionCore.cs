using System;
using System.Diagnostics.Contracts;
using System.Linq;
using Malsys.Evaluators;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Compilers.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FunctionCore {

		public const int AnyParamsCount = int.MaxValue;


		public readonly string Name;

		public readonly string Documentation;
		public readonly string Group;

		/// <summary>
		/// If function have params count equal to <c>AnyParamsCount</c> constant, any number of arguments can be supplied.
		/// </summary>
		public readonly int ParametersCount;

		/// <summary>
		/// Types of parameters of function.
		/// Length of the array do not have to be same length as function's arity.
		/// If function have more params than length of this array, modulo is used to cycle in it.
		/// </summary>
		public readonly ImmutableList<ExpressionValueTypeFlags> ParamsTypes;

		public readonly Func<ArgsStorage, IValue> EvalFunction;


		internal FunctionCore(string name, string group, string doc, int paramsCount, ExpressionValueTypeFlags[] paramsTypes,
				Func<ArgsStorage, IValue> evalFunc)
			: this(name, group, doc, paramsCount, new ImmutableList<ExpressionValueTypeFlags>(paramsTypes, true), evalFunc) {
		}

		public FunctionCore(string name, string group, string doc, int paramsCount, ImmutableList<ExpressionValueTypeFlags> paramsTypes,
				Func<ArgsStorage, IValue> evalFunc) {

			Contract.Requires<ArgumentNullException>(name != null);
			Contract.Requires<ArgumentOutOfRangeException>(paramsCount >= 0);
			Contract.Requires<ArgumentException>(paramsTypes != null);
			Contract.Requires<ArgumentNullException>(evalFunc != null);

			Name = name;
			Documentation = doc;
			Group = group;
			ParametersCount = paramsCount;
			ParamsTypes = paramsTypes;
			EvalFunction = evalFunc;
		}


		public FunctionCore ChangeNameTo(string newName) {
			return new FunctionCore(newName, Group, Documentation, ParametersCount, ParamsTypes, EvalFunction);
		}
	}
}

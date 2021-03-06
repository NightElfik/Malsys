﻿using System;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	/// <summary>
	/// Object for storing the function name and evaluation delegate and metadata.
	/// </summary>
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class FunctionInfo {

		/// <summary>
		/// Constant representing any parameters count on function.
		/// </summary>
		/// <remarks>
		/// Yet another Microsoft fail. I wanted to place this constant in interface BUT:
		/// In CIL it's possible to have const in interface, but C# does not support it.
		/// </remarks>
		public const int AnyParamsCount = int.MaxValue;

		public static readonly ImmutableList<ExpressionValueTypeFlags> AnyParamsTypes
			= new ImmutableList<ExpressionValueTypeFlags>(ExpressionValueTypeFlags.Any);

		public static readonly ImmutableList<ExpressionValueTypeFlags> ConstantParamsTypes
			= new ImmutableList<ExpressionValueTypeFlags>(ExpressionValueTypeFlags.Constant);

		public static readonly ImmutableList<ExpressionValueTypeFlags> ArrayParamsTypes
			= new ImmutableList<ExpressionValueTypeFlags>(ExpressionValueTypeFlags.Array);



		public readonly string Name;

		/// <summary>
		/// Function's parameters count. Use <c>FunctionInfo.AnyParamsCount</c>
		/// constant if function accepts any number of parameters.
		/// </summary>
		public readonly int ParamsCount;

		public readonly Func<IValue[], IExpressionEvaluatorContext, IValue> FunctionBody;

		/// <summary>
		/// Cyclic pattern of types of arguments (it can have different length than number of arguments).
		/// Use method <c>GetValueType</c> for getting correct type for parameter.
		/// </summary>
		public readonly ImmutableList<ExpressionValueTypeFlags> ParamsTypesCyclicPattern;

		public readonly object Metadata;

		public readonly string SummaryDoc;
		public readonly string GroupDoc;


		public FunctionInfo(string name, int paramsCount, Func<IValue[], IExpressionEvaluatorContext, IValue> functionBody,
			   ImmutableList<ExpressionValueTypeFlags> paramsTypesCyclicPattern, object metadata = null, string summaryDoc = null, string groupDoc = null) {

			Name = name;
			ParamsCount = paramsCount;
			FunctionBody = functionBody;
			ParamsTypesCyclicPattern = paramsTypesCyclicPattern;
			Metadata = metadata;
			SummaryDoc = summaryDoc;
			GroupDoc = groupDoc;
		}


		/// <param name="paramIndex">Zero-based parameter index.</param>
		public ExpressionValueTypeFlags GetParameterType(int paramIndex) {
			return ParamsTypesCyclicPattern[paramIndex % ParamsTypesCyclicPattern.Length];
		}


	}
}

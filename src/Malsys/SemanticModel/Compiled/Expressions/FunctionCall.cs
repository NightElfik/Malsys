﻿using System;
using Malsys.Evaluators;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FunctionCall : IExpression {

		public readonly string Name;

		public readonly ImmutableList<IExpression> Arguments;
		/// <summary>
		/// Describes cyclic pattern of types of arguments.
		/// Can have different length than Arguments list.
		/// Use function <c>GetValueType</c> to get right type for argument.
		/// </summary>
		public readonly ImmutableList<ExpressionValueType> ParamsTypes;

		public readonly Func<ArgsStorage, IValue> Evaluate;


		public FunctionCall(string name, Func<ArgsStorage, IValue> evalFunc, ImmutableList<IExpression> args, ImmutableList<ExpressionValueType> prmsTypes) {
			Name = name;
			Evaluate = evalFunc;
			Arguments = args;
			ParamsTypes = prmsTypes;
		}

		public ExpressionValueType GetValueType(int argIndex) {
			return ParamsTypes[argIndex % ParamsTypes.Length];
		}



		public bool IsEmptyExpression { get { return false; } }


		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

	}
}

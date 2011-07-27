﻿
using System.Collections.Generic;
namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FunctionCall : IExpression, IExpressionVisitable {

		public readonly string Name;
		public readonly ImmutableList<IExpression> Arguments;
		/// <summary>
		/// Describes cyclic pattern of types of arguments.
		/// Can have different length than Arguments list.
		/// Use function <c>GetValueType</c> to get right type for argument.
		/// </summary>
		public readonly ImmutableList<ExpressionValueType> ParamsTypes;

		public readonly EvalDelegate Evaluate;


		public FunctionCall(string name, EvalDelegate evalFunc, IEnumerable<IExpression> args, IEnumerable<ExpressionValueType> prmsTypes) {
			Name = name;
			Evaluate = evalFunc;
			Arguments = new ImmutableList<IExpression>(args);
			ParamsTypes = new ImmutableList<ExpressionValueType>(prmsTypes);
		}

		public FunctionCall(string name, EvalDelegate evalFunc, ImmutableList<IExpression> args, ImmutableList<ExpressionValueType> prmsTypes) {
			Name = name;
			Evaluate = evalFunc;
			Arguments = args;
			ParamsTypes = prmsTypes;
		}

		public ExpressionValueType GetValueType(int argIndex) {
			return ParamsTypes[argIndex % ParamsTypes.Length];
		}


		#region IExpressionVisitable Members

		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

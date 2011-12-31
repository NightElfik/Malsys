using System;
using System.Diagnostics.Contracts;
using Malsys.Evaluators;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Compilers.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class OperatorCore {


		public readonly string Syntax;
		/// <summary>
		/// Arity -- how many arguments operator needs.
		/// Only unary and binary operators are supported.
		/// </summary>
		public readonly int Arity;

		/// <summary>
		/// Normal precedence.
		/// </summary>
		/// <remarks>
		/// If precedence and active precedence are the same, operator is right associative.
		/// If active precedence is higher, operator is left associative.
		/// </remarks>
		public readonly byte Precedence;
		/// <summary>
		/// Precedence while I am holding this operator and deciding weather i push it on stack or pop some others before push.
		/// Idea by Martin Mareš :)
		/// </summary>
		public readonly byte ActivePrecedence;

		/// <summary>
		/// Types of parameters of operator.
		/// Length of the array is equal to operator's arity.
		/// </summary>
		public readonly ImmutableList<ExpressionValueType> ParamsTypes;

		public readonly Func<ArgsStorage, IValue> EvalFunction;


		internal OperatorCore(string syntax, byte prec, byte activePrec, ExpressionValueType[] paramsTypes, Func<ArgsStorage, IValue> evalFunc)
			: this(syntax, prec, activePrec, new ImmutableList<ExpressionValueType>(paramsTypes, true), evalFunc) {
		}

		public OperatorCore(string syntax, byte prec, byte activePrec, ImmutableList<ExpressionValueType> paramsTypes, Func<ArgsStorage, IValue> evalFunc) {

			Contract.Requires<ArgumentNullException>(syntax != null);
			Contract.Requires<ArgumentNullException>(paramsTypes != null);
			Contract.Requires<ArgumentException>(paramsTypes.Length == 1 || paramsTypes.Length == 2);
			Contract.Requires<ArgumentNullException>(evalFunc != null);

			Syntax = syntax;
			Arity = paramsTypes.Length;
			EvalFunction = evalFunc;
			Precedence = prec;
			ParamsTypes = paramsTypes;
			ActivePrecedence = activePrec;
		}
	}
}

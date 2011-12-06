using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.SemanticModel.Evaluated;
using Malsys.SemanticModel.Compiled;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using FunsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.FunctionEvaledParams>;
using Malsys.SemanticModel;
using Microsoft.FSharp.Collections;

namespace Malsys.Evaluators {

	public interface IExpressionEvaluator {

		/// <summary>
		/// Evaluates given expression with given constants and functions.
		/// </summary>
		IValue Evaluate(IExpression expr, ConstsMap consts, FunsMap funs);

	}


	public static class IExpressionEvaluatorExtensions {

		private static readonly ConstsMap emptyConstsMap = MapModule.Empty<string, IValue>();
		private static readonly FunsMap emptyFunsMap = MapModule.Empty<string, FunctionEvaledParams>();


		/// <summary>
		/// Evaluates given expression with no defined constants nor functions.
		/// </summary>
		public static IValue Evaluate(this IExpressionEvaluator evaluator, IExpression expr) {
			return evaluator.Evaluate(expr, emptyConstsMap, emptyFunsMap);
		}

		/// <summary>
		/// Evaluates given list of expressions with no defined constants nor functions.
		/// </summary>
		public static ImmutableList<IValue> EvaluateList(this IExpressionEvaluator evaluator, ImmutableList<IExpression> exprs) {

			var result = new IValue[exprs.Length];

			for (int i = 0; i < exprs.Length; i++) {
				result[i] = evaluator.Evaluate(exprs[i], emptyConstsMap, emptyFunsMap);
			}

			return new ImmutableList<IValue>(result, true);
		}

		/// <summary>
		/// Evaluates immutable list of expressions with given constants and functions.
		/// </summary>
		public static ImmutableList<IValue> EvaluateList(this IExpressionEvaluator evaluator, ImmutableList<IExpression> exprs, ConstsMap consts, FunsMap funs) {

			var result = new IValue[exprs.Length];

			for (int i = 0; i < exprs.Length; i++) {
				result[i] = evaluator.Evaluate(exprs[i], consts, funs);
			}

			return new ImmutableList<IValue>(result, true);
		}

		/// <summary>
		/// Evaluates expression with given constants and functions as Constant.
		/// If result is not Constant, EvalException is thrown.
		/// </summary>
		public static Constant EvaluateAsConst(this IExpressionEvaluator evaluator, IExpression expr, ConstsMap consts, FunsMap funs) {

			var e = evaluator.Evaluate(expr, consts, funs);
			if (e.IsConstant) {
				return (Constant)e;
			}
			else {
				throw new EvalException("Excepted constant after evaluation.");
			}
		}

	}


}

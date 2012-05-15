/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Collections.Generic;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	/// <summary>
	/// All implementations should be immutable.
	/// </summary>
	public interface IExpressionEvaluatorContext {


		IExpressionEvaluator ExpressionEvaluator { get; }

		IValue Evaluate(IExpression expr);


		bool TryGetVariableValue(string name, out IValue value);

		bool TryEvaluateFunction(string name, IValue[] args, out IValue value);


		/// <summary>
		/// Adds variable to returned IExpressionEvaluatorContext.
		/// If variable with same name already exists it will be overwritten with new one.
		/// </summary>
		/// <returns>New IExpressionEvaluatorContext with added variable.</returns>
		IExpressionEvaluatorContext AddVariable(VariableInfo variable, bool rewrite = true);

		/// <summary>
		/// Adds function to returned IExpressionEvaluatorContext.
		/// If function with same name already exists it will be overwritten with new one.
		/// </summary>
		/// <returns>New IExpressionEvaluatorContext with added function.</returns>
		IExpressionEvaluatorContext AddFunction(FunctionInfo function, bool rewrite = true);


		IEnumerable<VariableInfo> GetAllStoredVariables();

		IEnumerable<FunctionInfo> GetAllStoredFunctions();


		IExpressionEvaluatorContext MergeWith(IExpressionEvaluatorContext other);

	}


	public static class IExpressionEvaluatorContextExtensions {

		public static IValue TryEvaluate(this IExpressionEvaluatorContext eec, IExpression expr, IMessageLogger logger, IValue returnValueOnError = null) {
			try {
				return eec.Evaluate(expr);
			}
			catch (EvalException ex) {
				logger.LogMessage(Message.EvaluationError, ex.GetFullMessage());
				return returnValueOnError;
			}
		}

		public static Constant EvaluateAsConst(this IExpressionEvaluatorContext eec, IExpression expr) {

			var val = eec.Evaluate(expr);
			if (val.IsConstant) {
				return (Constant)val;
			}
			else {
				throw new EvalException("Excepted constant after evaluation.");
			}

		}

		public static ImmutableList<IValue> EvaluateList(this IExpressionEvaluatorContext eec, ImmutableList<IExpression> exprs) {

			var result = new IValue[exprs.Length];

			for (int i = 0; i < exprs.Length; i++) {
				result[i] = eec.Evaluate(exprs[i]);
			}

			return new ImmutableList<IValue>(result, true);

		}

		public static IExpressionEvaluatorContext AddVariable(this IExpressionEvaluatorContext eec, string name, IValue value, Ast.IAstNode astNode = null, bool rewrite = true) {
			return eec.AddVariable(new VariableInfo(name, () => value, astNode), rewrite);
		}

		public static IExpressionEvaluatorContext AddVariable(this IExpressionEvaluatorContext eec, string name, IExpression value, Ast.IAstNode astNode = null, bool rewrite = true) {
			var val = eec.Evaluate(value);
			return eec.AddVariable(new VariableInfo(name, () => val, astNode), rewrite);
		}

		internal static IExpressionEvaluatorContext AddFunction(this IExpressionEvaluatorContext eec, FunctionData fun, bool rewrite = true) {

			int mandatory = 0;
			for (int i = 0; i < fun.Parameters.Length; i++) {
				if (fun.Parameters[i].IsOptional) {
					break;
				}
				mandatory++;
			}

			for (int i = mandatory; i <= fun.Parameters.Length; i++) {
				eec = eec.AddFunction(new FunctionInfo(fun.Name, i, fun.evalUserFuncCall, FunctionInfo.AnyParamsTypes, fun), rewrite);
			}

			return eec;

		}


		public enum Message {

			[Message(MessageType.Error, "Failed to evaluate expression. {0}")]
			EvaluationError,

		}

	}
}

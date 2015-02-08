using System.Collections.Generic;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	/// <summary>
	/// Object for storing evaluated data of function call.
	/// This object is used in anonymous delegate as evaluate function when adding new function to the IExpressionEvaluatorContext.
	/// </summary>
	public class FunctionData {

		public string Name;
		public List<OptionalParameterEvaled> Parameters;
		public List<IFunctionStatement> Statements;


		/// <summary>
		/// Evaluates given function call with given arguments. Result is left on values stack.
		/// </summary>
		public IValue EvalUserFuncCall(IValue[] args, IExpressionEvaluatorContext exprEvalCtxt) {

			// Add arguments as new constants.
			for (int i = 0; i < Parameters.Count; i++) {
				IValue value;
				if (Parameters[i].IsOptional) {
					value = i < args.Length ? value = args[i] : Parameters[i].DefaultValue;
				}
				else {
					if (i < args.Length) {
						value = args[i];
					}
					else {
						throw new EvalException("Failed to evaluate function `{0}`. {1}. parameter is not optional and only {2} values given in function call."
							.Fmt(Name, i + 1, args.Length));
					}
				}

				exprEvalCtxt = exprEvalCtxt.AddVariable(Parameters[i].Name, value, Parameters[i].AstNode);
			}

			for (int i = 0; i < Statements.Count; i++) {

				var stat = Statements[i];
				switch (stat.StatementType) {

					case FunctionStatementType.ConstantDefinition:
						var cst = (ConstantDefinition)stat;
						var value = exprEvalCtxt.Evaluate(cst.Value);
						exprEvalCtxt = exprEvalCtxt.AddVariable(cst.Name, value, cst.AstNode);
						break;

					case FunctionStatementType.ReturnExpression:
						var retVal = (FunctionReturnExpr)stat;
						return exprEvalCtxt.Evaluate(retVal.ReturnValue);

					default:
						throw new EvalException("Unknown function's statement type `{0}` in function `{1}`."
							.Fmt(stat.StatementType, Name));

				}
			}

			throw new EvalException("Missing return statement in function `{0}`.".Fmt(Name));
		}


	}
}

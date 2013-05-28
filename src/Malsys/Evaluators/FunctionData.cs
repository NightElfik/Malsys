// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	/// <summary>
	/// Object for storing evaluated data of function call.
	/// This object is used in anonymous delegate as evaluate function when adding new function to the IExpressionEvaluatorContext.
	/// </summary>
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class FunctionData {

		public readonly string Name;
		public readonly ImmutableList<OptionalParameterEvaled> Parameters;
		public readonly ImmutableList<IFunctionStatement> Statements;


		public FunctionData(string name, ImmutableList<OptionalParameterEvaled> prms, ImmutableList<IFunctionStatement> stats) {

			Name = name;
			Parameters = prms;
			Statements = stats;

		}

		/// <summary>
		/// Evaluates given function call with given arguments. Result is left on values stack.
		/// </summary>
		public IValue evalUserFuncCall(IValue[] args, IExpressionEvaluatorContext exprEvalCtxt) {

			// add arguments as new constants
			for (int i = 0; i < Parameters.Length; i++) {
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

			for (int i = 0; i < Statements.Length; i++) {

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

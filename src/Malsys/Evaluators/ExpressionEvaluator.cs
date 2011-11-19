using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using FunsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.FunctionEvaledParams>;

namespace Malsys.Evaluators {
	public class ExpressionEvaluator {

		private static readonly ConstsMap emptyConstsMap = MapModule.Empty<string, IValue>();
		private static readonly FunsMap emptyFunsMap = MapModule.Empty<string, FunctionEvaledParams>();


		private ExpressionEvalVisitor evalVisitor = new ExpressionEvalVisitor();



		/// <summary>
		/// Evaluates expression with no defined variables nor functions.
		/// </summary>
		public IValue Evaluate(IExpression expr) {
			return Evaluate(expr, emptyConstsMap, emptyFunsMap);
		}

		/// <summary>
		/// Evaluates immutable list of expressions with no defined variables nor functions.
		/// </summary>
		public ImmutableList<IValue> Evaluate(ImmutableList<IExpression> exprs) {
			var result = new IValue[exprs.Length];

			for (int i = 0; i < exprs.Length; i++) {
				result[i] = Evaluate(exprs[i]);
			}

			return new ImmutableList<IValue>(result, true);
		}

		/// <summary>
		/// Evaluates expression with given variables and functions.
		/// </summary>
		public IValue Evaluate(IExpression expr, ConstsMap consts, FunsMap funs) {
			return evalVisitor.Evaluate(expr, consts, funs);
		}

		/// <summary>
		/// Evaluates immutable list of expressions with given variables and functions.
		/// </summary>
		public ImmutableList<IValue> Evaluate(ImmutableList<IExpression> exprs, ConstsMap consts, FunsMap funs) {
			var result = new IValue[exprs.Length];

			for (int i = 0; i < exprs.Length; i++) {
				result[i] = Evaluate(exprs[i], consts, funs);
			}

			return new ImmutableList<IValue>(result, true);
		}

		/// <summary>
		/// Evaluates expression with given variables and functions as Constant.
		/// If result of given expression is not Constant, EvalException is thrown.
		/// </summary>
		public Constant EvaluateAsConst(IExpression expr, ConstsMap consts, FunsMap funs) {

			var e = Evaluate(expr, consts, funs);
			if (e.IsConstant) {
				return (Constant)e;
			}
			else {
				throw new EvalException("Excpected constant after evaluation.");
			}
		}

		/// <summary>
		/// Evaluates optional params list with given variables and functions.
		/// </summary>
		public ImmutableList<OptionalParameterEvaled> EvaluateOptParams(ImmutableList<OptionalParameter> prms,
				ConstsMap consts, FunsMap funs) {

			return evalVisitor.EvaluateParameters(prms, consts, funs);
		}

		public Symbol<IValue> EvaluateSymbol(Symbol<IExpression> symbol, ConstsMap consts, FunsMap funs) {

			return new Symbol<IValue>(symbol.Name, Evaluate(symbol.Arguments, consts, funs));
		}

		public ImmutableList<Symbol<IValue>> EvaluateSymbols(ImmutableList<Symbol<IExpression>> symbols,
				ConstsMap consts, FunsMap funs) {

			var result = new Symbol<IValue>[symbols.Length];

			for (int i = 0; i < symbols.Length; i++) {
				var args = Evaluate(symbols[i].Arguments, consts, funs);
				result[i] = new Symbol<IValue>(symbols[i].Name, args);
			}

			return new ImmutableList<Symbol<IValue>>(result, true);
		}
	}
}

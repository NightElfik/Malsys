using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.FunctionEvaledParams>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;

namespace Malsys.Evaluators {
	public class ExpressionEvaluator {

		private static readonly VarMap emptyVarMap = MapModule.Empty<string, IValue>();
		private static readonly FunMap emptyFunMap = MapModule.Empty<string, FunctionEvaledParams>();


		private ExpressionEvalVisitor evalVisitor = new ExpressionEvalVisitor();



		/// <summary>
		/// Evaluates expression with no defined variables nor functions.
		/// </summary>
		public IValue Evaluate(IExpression expr) {
			return Evaluate(expr, emptyVarMap, emptyFunMap);
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
		public IValue Evaluate(IExpression expr, VarMap vars, FunMap funs) {
			return evalVisitor.Evaluate(expr, vars, funs);
		}

		/// <summary>
		/// Evaluates immutable list of expressions with given variables and functions.
		/// </summary>
		public ImmutableList<IValue> Evaluate(ImmutableList<IExpression> exprs, VarMap vars, FunMap funs) {
			var result = new IValue[exprs.Length];

			for (int i = 0; i < exprs.Length; i++) {
				result[i] = Evaluate(exprs[i], vars, funs);
			}

			return new ImmutableList<IValue>(result, true);
		}

		/// <summary>
		/// Evaluates expression with given variables and functions as Constant.
		/// If result of given expression is not Constant, EvalException is thrown.
		/// </summary>
		public Constant EvaluateAsConst(IExpression expr, VarMap vars, FunMap funs) {

			var e = Evaluate(expr, vars, funs);
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
		public ImmutableList<OptionalParameterEvaled> EvaluateOptParams(ImmutableList<OptionalParameter> prms, VarMap vars, FunMap funs) {
			return evalVisitor.EvaluateParameters(prms, vars, funs);
		}

		public Symbol<IValue> EvaluateSymbol(Symbol<IExpression> symbol, VarMap vars, FunMap funs) {

			return new Symbol<IValue>(symbol.Name, Evaluate(symbol.Arguments, vars, funs));
		}

		public SymbolsListVal EvaluateSymbols(SymbolsListExpr symbols, VarMap vars, FunMap funs) {

			var result = new Symbol<IValue>[symbols.Length];

			for (int i = 0; i < symbols.Length; i++) {
				var args = Evaluate(symbols[i].Arguments, vars, funs);
				result[i] = new Symbol<IValue>(symbols[i].Name, args);
			}

			return new SymbolsListVal(new ImmutableList<Symbol<IValue>>(result, true));
		}
	}
}

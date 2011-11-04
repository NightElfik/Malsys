using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Expressions {
	public static class SymbolEvaluator {

		public static Symbol<IValue> Evaluate(this Symbol<IExpression> symbol) {
			var args = ExpressionEvaluator.Evaluate(symbol.Arguments);
			return new Symbol<IValue>(symbol.Name, args);
		}

		public static Symbol<IValue> Evaluate(this Symbol<IExpression> symbol, VarMap vars, FunMap funs) {
			var args = ExpressionEvaluator.Evaluate(symbol.Arguments, vars, funs);
			return new Symbol<IValue>(symbol.Name, args);
		}

		public static SymbolsList<IValue> Evaluate(this SymbolsList<IExpression> symbols) {

			var result = new Symbol<IValue>[symbols.Length];

			for (int i = 0; i < symbols.Length; i++) {
				result[i] = Evaluate(symbols[i]);
			}

			return new SymbolsList<IValue>(new ImmutableList<Symbol<IValue>>(result, true));
		}

		public static SymbolsList<IValue> Evaluate(this SymbolsList<IExpression> symbols, VarMap vars, FunMap funs) {

			var result = new Symbol<IValue>[symbols.Length];

			for (int i = 0; i < symbols.Length; i++) {
				result[i] = Evaluate(symbols[i], vars, funs);
			}

			return new SymbolsList<IValue>(new ImmutableList<Symbol<IValue>>(result, true));
		}

	}
}

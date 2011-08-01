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

	}
}

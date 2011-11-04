﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.FSharp.Collections;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Expressions {
	public static class ExpressionEvaluator {

		[ThreadStatic]
		private static ExpressionEvalVisitor evalVisitor;


		/// <summary>
		/// Evaluates expression with no defined variables nor functions.
		/// Thread safe.
		/// </summary>
		public static IValue Evaluate(IExpression expr) {
			return Evaluate(expr, MapModule.Empty<string, IValue>(), MapModule.Empty<string, FunctionDefinition>());
		}

		/// <summary>
		/// Evaluates immutable list of expressions with no defined variables nor functions.
		/// Thread safe.
		/// </summary>
		public static ImmutableList<IValue> Evaluate(ImmutableList<IExpression> exprs) {
			var result = new IValue[exprs.Length];

			for (int i = 0; i < exprs.Length; i++) {
				result[i] = Evaluate(exprs[i]);
			}

			return new ImmutableList<IValue>(result, true);
		}

		/// <summary>
		/// Evaluates expression with given variables and functions.
		/// Thread safe.
		/// </summary>
		public static IValue Evaluate(IExpression expr, VarMap vars, FunMap funs) {

			if (evalVisitor == null) {
				evalVisitor = new ExpressionEvalVisitor();
			}

			return evalVisitor.Evaluate(expr, vars, funs);
		}

		/// <summary>
		/// Evaluates immutable list of expressions with given variables and functions.
		/// Thread safe.
		/// </summary>
		public static ImmutableList<IValue> Evaluate(ImmutableList<IExpression> exprs, VarMap vars, FunMap funs) {
			var result = new IValue[exprs.Length];

			for (int i = 0; i < exprs.Length; i++) {
				result[i] = Evaluate(exprs[i], vars, funs);
			}

			return new ImmutableList<IValue>(result, true);
		}

		/// <summary>
		/// Evaluates expression with given variables and functions as Constant.
		/// If result of given expression is not Constant, EvalException is thrown.
		/// Thread safe.
		/// </summary>
		public static Constant EvaluateAsConst(IExpression expr, VarMap vars, FunMap funs) {

			var e = Evaluate(expr, vars, funs);
			if (e.IsConstant) {
				return (Constant)e;
			}
			else {
				throw new EvalException("Excpected constant after evaluation.");
			}
		}
	}
}

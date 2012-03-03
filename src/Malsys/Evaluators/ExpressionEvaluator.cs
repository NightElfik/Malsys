using System;
using System.Collections.Generic;
using System.Diagnostics;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Compiled.Expressions;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using FunsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.FunctionEvaledParams>;

namespace Malsys.Evaluators {
	internal class ExpressionEvaluator : IExpressionEvaluator, IExpressionVisitor {

		private bool evaluating = false;
		private Stack<IValue> valuesStack = new Stack<IValue>();
		private ConstsMap constants;
		private FunsMap functions;

		private ArgsStorage argsStorage = new ArgsStorage();


		/// <summary>
		/// Evaluates given expression with respect to given constants and functions.
		/// </summary>
		/// <remarks>
		/// To enhance performance, entire instance uses one 'global' stack to do evaluation.
		/// Because of that it is not thread safe and moreover it is not 'recursion' safe.
		/// Simply this class can not call this public method from any methods called by it (recursion).
		/// If it happens, stack will contain some expressions and at the end there will be not enough operators to
		/// compose them and exception will be thrown.
		/// To avoid this 'danger', simple assert is in place.
		///
		/// Another little problem is with constants/funs, but they can be saved/restored easily.
		///
		/// To enable recursion, stack should implement 'moving' of its bottom, at start of call, bottom will be
		/// placed at current top, recurrent computation will see 'empty' stack for itself, and after returning,
		/// bottom will be placed on previous position, just like calls of real methods with real stack.
		/// Or use stack of stacks. It is probably easier to create another instance of this visitor.
		///
		/// But I think, that this feature is not necessary now.
		/// </remarks>
		public IValue Evaluate(IExpression expr, ConstsMap consts, FunsMap funs) {

			Debug.Assert(!evaluating, "Forbidden recursion call!");
			constants = consts;
			functions = funs;

			try {
				evaluating = true;

				expr.Accept(this);

				if (valuesStack.Count != 1) {
					throw new EvalException("Failed to evaluate expression. Not all values were processed.");
				}

				return valuesStack.Pop();
			}
			finally {
				evaluating = false;
				// cleanup
				valuesStack.Clear();
				constants = null;
				functions = null;
			}
		}


		#region IExpressionVisitor Members

		public void Visit(BinaryOperator binaryOperator) {

			binaryOperator.LeftOperand.Accept(this);

			if (!valuesStack.Peek().Type.IsCompatibleWith(binaryOperator.LeftOperandType)) {
				throw new EvalException("Failed to evaluate binary operator `{0}`. As left operand excepted {1}, but {2} was given."
					.Fmt(binaryOperator.Syntax, binaryOperator.LeftOperandType.ToTypeString(), valuesStack.Peek().Type.ToTypeString()));
			}

			binaryOperator.RightOperand.Accept(this);

			if (!valuesStack.Peek().Type.IsCompatibleWith(binaryOperator.LeftOperandType)) {
				throw new EvalException("Failed to evaluate binary operator `{0}`. As right operand excepted {1}, but {2} was given."
					.Fmt(binaryOperator.Syntax, binaryOperator.LeftOperandType.ToTypeString(), valuesStack.Peek().Type.ToTypeString()));
			}

			argsStorage.PopArgs(2, valuesStack);

			valuesStack.Push(binaryOperator.Evaluate(argsStorage));
		}

		public void Visit(Constant constant) {
			valuesStack.Push(constant);
		}

		public void Visit(EmptyExpression emptyExpression) {
			valuesStack.Push(Constant.NaN);
		}

		public void Visit(ExpressionValuesArray expressionValuesArray) {

			var arr = new IValue[expressionValuesArray.Length];

			for (int i = 0; i < arr.Length; i++) {
				expressionValuesArray[i].Accept(this);
				arr[i] = valuesStack.Pop();
			}

			var rsltImm = new ImmutableList<IValue>(arr, true);
			valuesStack.Push(new ValuesArray(rsltImm, expressionValuesArray.AstNode));
		}

		public void Visit(ExprVariable variable) {

			var maybeValue = MapModule.TryFind(variable.Name, constants);

			if (OptionModule.IsSome(maybeValue)) {
				valuesStack.Push(maybeValue.Value);
			}
			else {
				throw new EvalException("Unknown variable `{0}`.".Fmt(variable.Name));
			}
		}

		public void Visit(FunctionCall functionCall) {

			if (functionCall.Name.Equals("if", StringComparison.CurrentCultureIgnoreCase)) {
				evalIf(functionCall);
				return;
			}

			// evaluate arguments
			for (int i = 0; i < functionCall.Arguments.Length; i++) {

				functionCall.Arguments[i].Accept(this);

				if (!valuesStack.Peek().Type.IsCompatibleWith(functionCall.GetValueType(i))) {
					throw new EvalException("Failed to evaluate function `{0}`. As {1}. argument excepted {2}, but {3} was given."
						.Fmt(functionCall.Name, i, functionCall.GetValueType(i).ToTypeString(), valuesStack.Peek().Type.ToTypeString()));
				}
			}

			argsStorage.PopArgs(functionCall.Arguments.Length, valuesStack);
			valuesStack.Push(functionCall.Evaluate(argsStorage));
		}

		public void Visit(Indexer indexer) {

			indexer.Array.Accept(this);

			if (valuesStack.Peek().Type != ExpressionValueType.Array) {
				throw new EvalException("Failed to evaluate indexer. As operand excepted {0}, but {1} was given."
					.Fmt(ExpressionValueType.Array.ToTypeString(), valuesStack.Peek().Type.ToTypeString()));
			}
			ValuesArray arr = (ValuesArray)valuesStack.Pop();


			indexer.Index.Accept(this);

			if (valuesStack.Peek().Type != ExpressionValueType.Constant) {
				throw new EvalException("Failed to evaluate indexer. As index excepted {0}, but {1} was given."
					.Fmt(ExpressionValueType.Constant.ToTypeString(), valuesStack.Peek().Type.ToTypeString()));
			}

			Constant index = (Constant)valuesStack.Pop();
			if (index.IsNaN) {
				throw new EvalException("Failed to evaluate indexer, index is NaN.");
			}

			int intIndex = index.RoundedIntValue;

			if (intIndex < 0) {
				throw new EvalException("Failed to evaluate indexer, index out of range. Index is zero-based but negative value `{0}` was given."
					.Fmt(intIndex));
			}

			if (intIndex >= arr.Length) {
				throw new EvalException("Failed to evaluate indexer, index out of range. Can not index array of length {0} with zero-based index {1}."
					.Fmt(arr.Length, intIndex));
			}

			valuesStack.Push(arr[intIndex]);
		}

		public void Visit(UnaryOperator unaryOperator) {

			unaryOperator.Operand.Accept(this);

			if (!valuesStack.Peek().Type.IsCompatibleWith(unaryOperator.OperandType)) {
				throw new EvalException("Failed to evaluate unary operator `{0}`. As operand excepted {1}, but {2} was given."
					.Fmt(unaryOperator.Syntax, unaryOperator.OperandType.ToTypeString(), valuesStack.Peek().Type.ToTypeString()));
			}

			argsStorage.PopArgs(1, valuesStack);

			valuesStack.Push(unaryOperator.Evaluate(argsStorage));
		}

		public void Visit(UserFunctionCall userFunction) {

			var maybeFun = MapModule.TryFind(userFunction.Name, functions);

			if (!OptionModule.IsSome(maybeFun)) {
				throw new EvalException("Unknown function `{0}`.".Fmt(userFunction.Name));
			}

			var fun = maybeFun.Value;

			if (userFunction.Arguments.Length > fun.Parameters.Length) {
				throw new EvalException("Failed to evaluate function `{0}`. It takes only {1} parameters but {2} arguments were given."
					.Fmt(userFunction.Name, fun.Parameters.Length, userFunction.Arguments.Length));
			}

			// evaluate arguments
			for (int i = 0; i < userFunction.Arguments.Length; i++) {
				userFunction.Arguments[i].Accept(this);
			}

			argsStorage.PopArgs(userFunction.Arguments.Length, valuesStack);

			evalUserFuncCall(fun, argsStorage);
		}

		#endregion


		/// <summary>
		/// Evaluates given function call with given arguments. Result is left on values stack.
		/// </summary>
		private void evalUserFuncCall(FunctionEvaledParams fun, ArgsStorage args) {
			// save variables & functions
			var oldConsts = constants;
			var oldFuns = functions;

			// add arguments as new constants
			for (int i = 0; i < fun.Parameters.Length; i++) {
				IValue value;
				if (fun.Parameters[i].IsOptional) {
					value = i < args.ArgsCount ? value = args[i] : fun.Parameters[i].DefaultValue;
				}
				else {
					if (i < args.ArgsCount) {
						value = args[i];
					}
					else {
						throw new EvalException("Failed to evaluate function `{0}`. {1}. parameter is not optional and only {2} values given in function call."
							.Fmt(fun.AstNode.NameId.Name, i + 1, args.ArgsCount));
					}
				}

				constants = constants.Add(fun.Parameters[i].Name, value);
			}

			for (int i = 0; i < fun.Statements.Length; i++) {

				var stat = fun.Statements[i];
				switch (stat.StatementType) {
					case FunctionStatementType.ConstantDefinition:
						var cst = (ConstantDefinition)stat;
						cst.Value.Accept(this);
						constants = constants.Add(cst.Name, valuesStack.Pop());
						break;

					case FunctionStatementType.ReturnExpression:
						// function's return value
						var retVal = (FunctionReturnExpr)stat;
						retVal.ReturnValue.Accept(this);
						goto breakFor;

					default:
						throw new EvalException("Unknown function's statement type `{0}` in function `{1}`."
							.Fmt(stat.StatementType, fun.AstNode.NameId.Name));

				}
			}

		breakFor:
			// restore variables & functions to state before fun call
			constants = oldConsts;
			functions = oldFuns;
		}

		private void evalIf(FunctionCall fun) {

			fun.Arguments[0].Accept(this);

			var cond = valuesStack.Pop();
			if (!cond.IsConstant) {
				throw new EvalException("Failed to evaluate function `{0}`. As first argument excepted {1}, but {2} was given."
					.Fmt(fun.Name, (ExpressionValueType.Constant).ToTypeString(), cond.Type.ToTypeString()));
			}

			IExpression arg;

			if (!FloatArithmeticHelper.IsZero((Constant)cond)) {
				// not zero -- true
				arg = fun.Arguments[1];
			}
			else {
				// zero -- false
				arg = fun.Arguments[2];
			}

			arg.Accept(this);
		}

	}
}

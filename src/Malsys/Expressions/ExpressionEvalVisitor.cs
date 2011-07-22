using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Expressions {
	public class ExpressionEvalVisitor : IExpressionVisitor {

		private Stack<IValue> valuesStack = new Stack<IValue>();
		private VarMap variables;
		private FunMap functions;

		private ArgsStorage argsStorage = new ArgsStorage();



		public IValue Evaluate(IExpression expr, VarMap vars, FunMap funs) {

			variables = vars;
			functions = funs;

			expr.Accept(this);

			variables = null;
			functions = null;
			argsStorage.Clear();

			if (valuesStack.Count != 1) {
				valuesStack.Clear();
				throw new EvalException("Failed to evaluate expression. Not all values were processed.");
			}

			return valuesStack.Pop();  // Peek (not Pop) to be able to call for result more than once.
		}


		#region IExpressionVisitor Members

		public void Visit(Constant constant) {
			valuesStack.Push(constant);
		}

		public void Visit(Variable variable) {
			var maybeValue = MapModule.TryFind(variable.Name, variables);

			if (OptionModule.IsSome(maybeValue)) {
				valuesStack.Push(maybeValue.Value);
			}
			else {
				throw new EvalException("Unknown variable `{0}`.".Fmt(variable.Name));
			}
		}

		public void Visit(ExpressionValuesArray expressionValuesArray) {
			IValue[] arr = new IValue[expressionValuesArray.Length];

			for (int i = 0; i < arr.Length; i++) {
				expressionValuesArray[i].Accept(this);
				arr[i] = valuesStack.Pop();
			}

			valuesStack.Push(new ValuesArray(arr));
		}

		public void Visit(UnaryOperator unaryOperator) {
			unaryOperator.Operand.Accept(this);
			if ((int)(valuesStack.Peek().Type & unaryOperator.OperandType) == 0) {
				throw new EvalException("Failed to evaluate unary operator `{0}`. As operand excpected {1}, but {2} was given.".Fmt(
					unaryOperator.Syntax, unaryOperator.OperandType.ToTypeString(), valuesStack.Peek().Type.ToTypeString()));
			}

			argsStorage.PopArgs(1, valuesStack);

			valuesStack.Push(unaryOperator.Evaluate(argsStorage));
		}

		public void Visit(BinaryOperator binaryOperator) {
			binaryOperator.LeftOperand.Accept(this);
			if ((int)(valuesStack.Peek().Type & binaryOperator.LeftOperandType) == 0) {
				throw new EvalException("Failed to evaluate binary operator `{0}`. As left operand excpected {1}, but {2} was given.".Fmt(
					binaryOperator.Syntax, binaryOperator.LeftOperandType.ToTypeString(), valuesStack.Peek().Type.ToTypeString()));
			}

			binaryOperator.RightOperand.Accept(this);
			if ((int)(valuesStack.Peek().Type & binaryOperator.LeftOperandType) == 0) {
				throw new EvalException("Failed to evaluate binary operator `{0}`. As right operand excpected {1}, but {2} was given.".Fmt(
					binaryOperator.Syntax, binaryOperator.LeftOperandType.ToTypeString(), valuesStack.Peek().Type.ToTypeString()));
			}

			argsStorage.PopArgs(2, valuesStack);

			valuesStack.Push(binaryOperator.Evaluate(argsStorage));
		}

		public void Visit(Indexer indexer) {
			indexer.Array.Accept(this);
			if (valuesStack.Peek().Type != ExpressionValueType.Array) {
				throw new EvalException("Failed to evaluate indexer. As operand excpected {0}, but {1} was given.".Fmt(
					ExpressionValueType.Array.ToTypeString(), valuesStack.Peek().Type.ToTypeString()));
			}
			ValuesArray arr = (ValuesArray)valuesStack.Pop();

			indexer.Index.Accept(this);
			if (valuesStack.Peek().Type != ExpressionValueType.Constant) {
				throw new EvalException("Failed to evaluate indexer. As index excpected {0}, but {1} was given.".Fmt(
					ExpressionValueType.Constant.ToTypeString(), valuesStack.Peek().Type.ToTypeString()));
			}

			Constant index = (Constant)valuesStack.Pop();

			valuesStack.Push(arr[(int)Math.Round(index)]);
		}

		public void Visit(Function function) {
			// evaluate arguments
			for (int i = 0; i < function.ArgumentsCount; i++) {
				function[i].Accept(this);
				if ((int)(valuesStack.Peek().Type & function.GetValueType(i)) == 0) {
					throw new EvalException("Failed to evaluate function `{0}`. As {1}. argument excpected {1}, but {2} was given.".Fmt(
						function.Name, i, function.GetValueType(i).ToTypeString(), valuesStack.Peek().Type.ToTypeString()));
				}
			}

			Debug.Assert(valuesStack.Count >= function.ArgumentsCount,
				"Excpected at least {0} values in stack, but it has only {1}.".Fmt(
					function.ArgumentsCount, valuesStack.Count));

			argsStorage.PopArgs(function.ArgumentsCount, valuesStack);
			function.Evaluate(argsStorage);
		}

		public void Visit(UserFunction userFunction) {
			var maybeFun = MapModule.TryFind(userFunction.Name, functions);

			if (!OptionModule.IsSome(maybeFun)) {
				throw new EvalException("Unknown function `{0}`.".Fmt(userFunction.Name));
			}

			var fun = maybeFun.Value;

			if (userFunction.ArgumentsCount > fun.ParametersCount) {
				throw new EvalException("Failed to evaluate function `{0}`. It takes only {1} parameters but {2} arguments were given.".Fmt(
					userFunction.Name, fun.ParametersCount, userFunction.ArgumentsCount));
			}

			if (userFunction.ArgumentsCount + fun.OptionalParamsCount < fun.ParametersCount) {
				throw new EvalException("Failed to evaluate function `{0}`. It takes {1} parameters, it have last {2} parameters optional but only {3} arguments were given.".Fmt(
					userFunction.Name, fun.ParametersCount, fun.OptionalParamsCount, userFunction.ArgumentsCount));
			}

			// evaluate arguments
			for (int i = 0; i < userFunction.ArgumentsCount; i++) {
				userFunction[i].Accept(this);
			}

			Debug.Assert(valuesStack.Count >= userFunction.ArgumentsCount,
				 "Excpected at least {0} values in stack, but it has only {1}.".Fmt(
					 userFunction.ArgumentsCount, valuesStack.Count));

			argsStorage.PopArgs(userFunction.ArgumentsCount, valuesStack);

			UserFunctionEvaluator.Evaluate(fun, argsStorage, variables, functions);
		}

		#endregion
	}
}

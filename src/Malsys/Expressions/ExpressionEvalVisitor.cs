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
			valuesStack.Clear();
			variables = vars;
			functions = funs;

			expr.Accept(this);

			variables = null;
			functions = null;
			argsStorage.Clear();

			if (valuesStack.Count != 1) {
				throw new EvalException("Failed to evaluate expression. Not all values were processed.");
			}

			return valuesStack.Pop();
		}

#if DEBUG
		/// <summary>
		/// Checks wether after Accept on given expression is exactly one more value on stack.
		/// </summary>
		private void checkedAccept(IExpression expr) {
			int oldStackCount = valuesStack.Count;

			expr.Accept(this);

			Debug.Assert(valuesStack.Count - oldStackCount == 1,
				"After Accept on expression excpected one more value on stack, but diff is {0}.".Fmt(
					valuesStack.Count - oldStackCount));
		}
#endif

		/// <summary>
		/// Evaluates given function call with given arguments. Result is left on velues stack.
		/// </summary>
		private void evalUserFuncCall(FunctionDefinition fun, ArgsStorage args) {
			// save variables
			var oldVars = variables;

			// add arguments as new vars
			for (int i = 0; i < fun.ParametersCount; i++) {
				IValue value = i < args.ArgsCount ? args[i] : fun.GetOptionalParamValue(i);
				variables = MapModule.Add(fun.ParametersNames[i], value, variables);
			}

			// evaluate variable definitions
			foreach (var varDef in fun.VariableDefinitions) {
#if DEBUG
				checkedAccept(varDef.Value);
#else
				varDef.Value.Accept(this);
#endif
				variables = MapModule.Add(varDef.Name, valuesStack.Pop(), variables);
			}

#if DEBUG
			checkedAccept(fun.Expression);
#else
			fun.Expression.Accept(this);
#endif
			// restore variables to state before fun call
			variables = oldVars;
		}

		private void evalIf(FunctionCall fun) {
#if DEBUG
			checkedAccept(fun.Arguments[0]);
#else
			fun.Arguments[0].Accept(this);
#endif

			var cond = valuesStack.Pop();
			if (!cond.IsConstant) {
				throw new EvalException("Failed to evaluate function `{0}`. As first argument excpected {1}, but {2} was given.".Fmt(
					fun.Name, (ExpressionValueType.Constant).ToTypeString(), cond.Type.ToTypeString()));
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

#if DEBUG
			checkedAccept(arg);
#else
			arg.Accept(this);
#endif
		}


		#region IExpressionVisitor Members

		public void Visit(Constant constant) {
			valuesStack.Push(constant);
		}

		public void Visit(ExprVariable variable) {
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
#if DEBUG
				checkedAccept(expressionValuesArray[i]);
#else
				expressionValuesArray[i].Accept(this);
#endif
				arr[i] = valuesStack.Pop();
			}

			var rsltImm = new ImmutableList<IValue>(arr, true);
			valuesStack.Push(new ValuesArray(rsltImm));
		}

		public void Visit(UnaryOperator unaryOperator) {
#if DEBUG
			checkedAccept(unaryOperator.Operand);
#else
			unaryOperator.Operand.Accept(this);
#endif
			if ((int)(valuesStack.Peek().Type & unaryOperator.OperandType) == 0) {
				throw new EvalException("Failed to evaluate unary operator `{0}`. As operand excpected {1}, but {2} was given.".Fmt(
					unaryOperator.Syntax, unaryOperator.OperandType.ToTypeString(), valuesStack.Peek().Type.ToTypeString()));
			}

			argsStorage.PopArgs(1, valuesStack);

			valuesStack.Push(unaryOperator.Evaluate(argsStorage));
		}

		public void Visit(BinaryOperator binaryOperator) {
#if DEBUG
			checkedAccept(binaryOperator.LeftOperand);
#else
			binaryOperator.LeftOperand.Accept(this); ;
#endif
			if ((int)(valuesStack.Peek().Type & binaryOperator.LeftOperandType) == 0) {
				throw new EvalException("Failed to evaluate binary operator `{0}`. As left operand excpected {1}, but {2} was given.".Fmt(
					binaryOperator.Syntax, binaryOperator.LeftOperandType.ToTypeString(), valuesStack.Peek().Type.ToTypeString()));
			}

#if DEBUG
			checkedAccept(binaryOperator.RightOperand);
#else
			binaryOperator.RightOperand.Accept(this);
#endif
			if ((int)(valuesStack.Peek().Type & binaryOperator.LeftOperandType) == 0) {
				throw new EvalException("Failed to evaluate binary operator `{0}`. As right operand excpected {1}, but {2} was given.".Fmt(
					binaryOperator.Syntax, binaryOperator.LeftOperandType.ToTypeString(), valuesStack.Peek().Type.ToTypeString()));
			}

			argsStorage.PopArgs(2, valuesStack);

			valuesStack.Push(binaryOperator.Evaluate(argsStorage));
		}

		public void Visit(Indexer indexer) {
#if DEBUG
			checkedAccept(indexer.Array);
#else
			indexer.Array.Accept(this);
#endif
			if (valuesStack.Peek().Type != ExpressionValueType.Array) {
				throw new EvalException("Failed to evaluate indexer. As operand excpected {0}, but {1} was given.".Fmt(
					ExpressionValueType.Array.ToTypeString(), valuesStack.Peek().Type.ToTypeString()));
			}
			ValuesArray arr = (ValuesArray)valuesStack.Pop();

#if DEBUG
			checkedAccept(indexer.Index);
#else
			indexer.Index.Accept(this);
#endif
			if (valuesStack.Peek().Type != ExpressionValueType.Constant) {
				throw new EvalException("Failed to evaluate indexer. As index excpected {0}, but {1} was given.".Fmt(
					ExpressionValueType.Constant.ToTypeString(), valuesStack.Peek().Type.ToTypeString()));
			}

			Constant index = (Constant)valuesStack.Pop();

			int intIndex = (int)Math.Round(index);

			if (intIndex < 0) {
				throw new EvalException("Failed to evaluate indexer, index out of range. Index is zero-based but negative value `{0}` was given.".Fmt(
					intIndex));
			}

			if (intIndex >= arr.Length) {
				throw new EvalException("Failed to evaluate indexer, index out of range. Can not index array of length {0} with zero-based index {1}.".Fmt(
					arr.Length, intIndex));
			}

			valuesStack.Push(arr[intIndex]);
		}

		public void Visit(FunctionCall functionCall) {

			if (functionCall.Evaluate == null && functionCall.Name.Equals("if", StringComparison.CurrentCultureIgnoreCase)) {
				evalIf(functionCall);
				return;
			}

			// evaluate arguments
			for (int i = 0; i < functionCall.Arguments.Length; i++) {
#if DEBUG
				checkedAccept(functionCall.Arguments[i]);
#else
				functionCall.Arguments[i].Accept(this);
#endif
				if ((int)(valuesStack.Peek().Type & functionCall.GetValueType(i)) == 0) {
					throw new EvalException("Failed to evaluate function `{0}`. As {1}. argument excpected {2}, but {3} was given.".Fmt(
						functionCall.Name, i, functionCall.GetValueType(i).ToTypeString(), valuesStack.Peek().Type.ToTypeString()));
				}
			}

			argsStorage.PopArgs(functionCall.Arguments.Length, valuesStack);
			valuesStack.Push(functionCall.Evaluate(argsStorage));
		}

		public void Visit(UserFunctionCall userFunction) {
			var maybeFun = MapModule.TryFind(userFunction.Name, functions);

			if (!OptionModule.IsSome(maybeFun)) {
				throw new EvalException("Unknown function `{0}`.".Fmt(userFunction.Name));
			}

			var fun = maybeFun.Value;

			if (userFunction.Arguments.Length > fun.ParametersCount) {
				throw new EvalException("Failed to evaluate function `{0}`. It takes only {1} parameters but {2} arguments were given.".Fmt(
					userFunction.Name, fun.ParametersCount, userFunction.Arguments.Length));
			}

			if (userFunction.Arguments.Length + fun.OptionalParamsCount < fun.ParametersCount) {
				throw new EvalException("Failed to evaluate function `{0}`. It takes {1} parameters, it have last {2} parameters optional but only {3} arguments were given.".Fmt(
					userFunction.Name, fun.ParametersCount, fun.OptionalParamsCount, userFunction.Arguments.Length));
			}

			// evaluate arguments
			for (int i = 0; i < userFunction.Arguments.Length; i++) {
#if DEBUG
				checkedAccept(userFunction.Arguments[i]);
#else
				userFunction.Arguments[i].Accept(this);
#endif
			}

			argsStorage.PopArgs(userFunction.Arguments.Length, valuesStack);

			evalUserFuncCall(fun, argsStorage);
		}

		#endregion
	}
}

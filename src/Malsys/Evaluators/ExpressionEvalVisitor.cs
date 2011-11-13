using System;
using System.Collections.Generic;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Compiled.Expressions;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.FunctionEvaled>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;

namespace Malsys.Evaluators {
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

		public FunctionEvaled EvaluateFunction(Function fun, VarMap vars, FunMap funs) {

			valuesStack.Clear();
			variables = vars;
			functions = funs;

			var result = evaluateFunction(fun);

			variables = null;
			functions = null;
			argsStorage.Clear();

			return result;
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

			var arr = new IValue[expressionValuesArray.Length];

			for (int i = 0; i < arr.Length; i++) {
				expressionValuesArray[i].Accept(this);
				arr[i] = valuesStack.Pop();
			}

			var rsltImm = new ImmutableList<IValue>(arr, true);
			valuesStack.Push(new ValuesArray(rsltImm));
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

			int intIndex = index.RoundedIntValue;

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

				functionCall.Arguments[i].Accept(this);

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

			if (userFunction.Arguments.Length > fun.Parameters.Length) {
				throw new EvalException("Failed to evaluate function `{0}`. It takes only {1} parameters but {2} arguments were given.".Fmt(
					userFunction.Name, fun.Parameters.Length, userFunction.Arguments.Length));
			}

			// evaluate arguments
			for (int i = 0; i < userFunction.Arguments.Length; i++) {
				userFunction.Arguments[i].Accept(this);
			}

			argsStorage.PopArgs(userFunction.Arguments.Length, valuesStack);

			evalUserFuncCall(fun, argsStorage);
		}

		#endregion


		private void evaluateBindings(ImmutableList<Binding> bindings, ref VarMap vars, ref FunMap funs) {

			foreach (var bind in bindings) {
				switch (bind.BindingType) {
					case BindingType.Expression:
						((IExpression)bind.Value).Accept(this);
						vars = vars.Add(bind.Name, valuesStack.Pop());
						break;
					case BindingType.Function:
						var fun = ((Function)bind.Value);
						funs = funs.Add(bind.Name, evaluateFunction(fun));
						break;
					case BindingType.SymbolList:
						throw new EvalException("Unexcpected symbols binding");
					default:
						throw new EvalException("Unknown binding");
				}
			}

		}

		private FunctionEvaled evaluateFunction(Function fun) {

			var prms = new OptionalParameterEvaled[fun.Parameters.Length];

			for (int i = 0; i < prms.Length; i++) {
				var currParam = fun.Parameters[i];

				if (currParam.IsOptional) {
					currParam.DefaultValue.Accept(this);
					prms[i] = new OptionalParameterEvaled(currParam.Name, valuesStack.Pop());
				}
				else {
					prms[i] = new OptionalParameterEvaled(currParam.Name);
				}
			}

			var prmsImm = new ImmutableList<OptionalParameterEvaled>(prms, true);
			return new FunctionEvaled(prmsImm, fun.Bindings, fun.ReturnExpression);
		}


		/// <summary>
		/// Evaluates given function call with given arguments. Result is left on velues stack.
		/// </summary>
		private void evalUserFuncCall(FunctionEvaled fun, ArgsStorage args) {
			// save variables & functions
			var oldVars = variables;
			var oldFuns = functions;

			// add arguments as new vars
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
						throw new EvalException("Failed to evaluate function `{0}`. {1}. parameter is not optional and only {2} values given in function call.".Fmt(
							fun.BindedName, i + 1, args.ArgsCount));
					}
				}

				variables = MapModule.Add(fun.Parameters[i].Name, value, variables);
			}

			evaluateBindings(fun.Bindings, ref variables, ref functions);

			fun.ReturnExpression.Accept(this);

			// restore variables & functions to state before fun call
			variables = oldVars;
			functions = oldFuns;
		}

		private void evalIf(FunctionCall fun) {

			fun.Arguments[0].Accept(this);

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

			arg.Accept(this);
		}

	}
}

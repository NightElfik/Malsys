using System;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	class BindingCompilerVisitor : Ast.IBindableVisitor {

		private MessagesCollection msgs;

		private InputCompiler inCompiler;
		private ExpressionCompiler exprCompiler;


		private AllowedBindingTypes allowedTypes;
		private IBindable result;
		private BindingType resultType;


		public BindingCompilerVisitor(InputCompiler comp) {

			msgs = comp.Messages;

			inCompiler = comp;
			exprCompiler = comp.ExpressionCompiler;
		}

		public CompilerResult<Binding> TryCompile(Ast.Binding binding, AllowedBindingTypes allowTypes) {

			allowedTypes = allowTypes;
			binding.Value.Accept(this);

			if (result != null) {
				return new Binding(binding.NameId.Name, result, resultType);
			}
			else {
				return CompilerResult<Binding>.Error;
			}
		}


		#region IBindableVisitor Members

		public void Visit(Ast.Expression expr) {

			if ((allowedTypes & AllowedBindingTypes.ExpressionsOnly) != 0) {
				result = exprCompiler.CompileExpression(expr);
				resultType = BindingType.Expression;
			}
			else {
				msgs.AddError("Expression binding is not possible in this context.", expr.Position);
				result = null;
			}
		}

		public void Visit(Ast.Function fun) {

			if ((allowedTypes & AllowedBindingTypes.FunctionsOnly) != 0) {
				var prms = inCompiler.CompileParametersFailSafe(fun.Parameters);
				var varDefs = inCompiler.CompileBindingsList(fun.LocalBindings, AllowedBindingTypes.ExpressionsAndFunctions);
				var retExpr = exprCompiler.CompileExpression(fun.ReturnExpression);

				result = new Function(prms, varDefs, retExpr);
				resultType = BindingType.Function;
			}
			else {
				msgs.AddError("Function binding is not possible in this context.", fun.Position);
				result = null;
			}
		}

		public void Visit(Ast.LsystemSymbolList symbolsList) {

			if ((allowedTypes & AllowedBindingTypes.SymbolListsOnly) != 0) {
				result = inCompiler.LsystemCompiler.CompileSymbolsList(symbolsList);
				resultType = BindingType.SymbolList;
			}
			else {
				msgs.AddError("Symbols binding is not possible in this context.", symbolsList.Position);
				result = null;
			}
		}

		#endregion
	}

	[Flags]
	public enum AllowedBindingTypes {
		ExpressionsOnly = 0x01,
		FunctionsOnly = 0x02,
		SymbolListsOnly = 0x04,

		ExpressionsAndFunctions = ExpressionsOnly | FunctionsOnly,
		All = ExpressionsOnly | FunctionsOnly | SymbolListsOnly
	}
}

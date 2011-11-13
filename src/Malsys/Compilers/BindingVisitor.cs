using System;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	class BindingCompilerVisitor : Ast.IBindableVisitor {

		private MessagesCollection msgs;

		private InputCompiler inCompiler;

		private Ast.Binding inBinding;
		private BindingType allowedTypes;

		private IBindable result;
		private BindingType resultType;


		public BindingCompilerVisitor(InputCompiler comp) {

			msgs = comp.Messages;

			inCompiler = comp;
		}

		public CompilerResult<Binding> TryCompile(Ast.Binding binding, BindingType allowTypes) {

			allowedTypes = allowTypes;
			inBinding = binding;
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

			if ((allowedTypes & BindingType.Expression) != 0) {
				result = inCompiler.ExpressionCompiler.CompileExpression(expr);
				resultType = BindingType.Expression;
			}
			else {
				msgs.AddError("Expression binding is not possible in this context.", expr.Position);
				result = null;
			}
		}

		public void Visit(Ast.Function fun) {

			if ((allowedTypes & BindingType.Function) != 0) {
				var bind = inBinding; // save, recursion can destroy it
				var prms = inCompiler.CompileParameters(fun.Parameters);
				var localBinds = inCompiler.CompileBindingsList(fun.LocalBindings, BindingType.ExpressionsAndFunctions);
				var retExpr = inCompiler.ExpressionCompiler.CompileExpression(fun.ReturnExpression);

				result = new Function(prms, localBinds, retExpr, bind);
				resultType = BindingType.Function;
			}
			else {
				msgs.AddError("Function binding is not possible in this context.", fun.Position);
				result = null;
			}
		}

		public void Visit(Ast.LsystemSymbolList symbolsList) {

			if ((allowedTypes & BindingType.SymbolList) != 0) {
				result = inCompiler.LsystemCompiler.CompileSymbolsList(symbolsList);
				resultType = BindingType.SymbolList;
			}
			else {
				msgs.AddError("Symbols binding is not possible in this context.", symbolsList.Position);
				result = null;
			}
		}

		public void Visit(Ast.Lsystem lsystem) {

			if ((allowedTypes & BindingType.Lsystem) != 0) {
				var bind = inBinding; // save, recursion can destroy it
				var prms = inCompiler.CompileParameters(lsystem.Parameters);
				var stats = inCompiler.LsystemCompiler.CompileLsystemStatements(lsystem.Statements);

				result = new Lsystem(prms, stats, bind);
				resultType = BindingType.Lsystem;
			}
			else {
				msgs.AddError("Lsystem binding is not possible in this context.", lsystem.Position);
				result = null;
			}
		}

		#endregion
	}
}

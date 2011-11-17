using System;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	class LsystemCompilerVisitor : Ast.IAstLsystemVisitor {

		private InputCompiler inCompiler;


		private CompilerResult<ILsystemStatement> result;


		public LsystemCompilerVisitor(InputCompiler inComp) {
			inCompiler = inComp;
		}

		public CompilerResult<ILsystemStatement> TryCompile(Ast.ILsystemStatement astStatement) {

			astStatement.Accept(this);
			return result;
		}


		#region IAstLsystemVisitor Members

		public void Visit(Ast.Binding binding) {
			var bind = inCompiler.CompileBinding(binding, BindingType.Expression | BindingType.Function | BindingType.SymbolList);
			result = new CompilerResult<ILsystemStatement>(bind.Result, bind.ErrorOcured);
		}

		public void Visit(Ast.EmptyStatement emptyStat) {
			result = CompilerResult<ILsystemStatement>.Error;
		}

		public void Visit(Ast.InterpretationBinding interpretBinding) {
			throw new NotImplementedException();
		}

		public void Visit(Ast.RewriteRule rewriteRule) {
			var rr = inCompiler.LsystemCompiler.CompileRewriteRule(rewriteRule);
			result = new CompilerResult<ILsystemStatement>(rr.Result, rr.ErrorOcured);
		}

		#endregion
	}
}

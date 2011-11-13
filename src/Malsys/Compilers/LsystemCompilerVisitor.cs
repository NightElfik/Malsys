using System;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	class LsystemCompilerVisitor : Ast.IAstLsystemVisitor {

		private InputCompiler inCompiler;


		private ILsystemStatement result;


		public LsystemCompilerVisitor(InputCompiler inComp) {
			inCompiler = inComp;
		}

		public CompilerResult<ILsystemStatement> TryCompile(Ast.ILsystemStatement astStatement) {

			astStatement.Accept(this);
			if (result != null) {
				return new CompilerResult<ILsystemStatement>(result);
			}
			else {
				return CompilerResult<ILsystemStatement>.Error;
			}
		}


		#region IAstLsystemVisitor Members

		public void Visit(Ast.Binding binding) {
			var bind = inCompiler.TryCompileBinding(binding, AllowedBindingTypes.All);
			result = bind ? (ILsystemStatement)bind : null;
		}

		public void Visit(Ast.EmptyStatement emptyStat) {
			result = null;
		}

		public void Visit(Ast.InterpretationBinding interpretBinding) {
			throw new NotImplementedException();
		}

		public void Visit(Ast.RewriteRule rewriteRule) {
			var rr = inCompiler.LsystemCompiler.CompileRewriteRule(rewriteRule);
			result = rr ? (ILsystemStatement)rr : null;
		}

		#endregion
	}
}

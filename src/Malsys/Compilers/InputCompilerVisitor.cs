using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	class InputCompilerVisitor : Ast.IAstInputVisitor {


		private InputCompiler inCompiler;


		private CompilerResult<IInputStatement> result;


		public InputCompilerVisitor(InputCompiler inComp) {
			inCompiler = inComp;
		}


		public CompilerResult<IInputStatement> TryCompile(Ast.IInputStatement astStatement) {

			astStatement.Accept(this);
			return result;
		}


		#region IAstInputVisitor Members

		public void Visit(Ast.Binding binding) {
			var bind = inCompiler.CompileBinding(binding, BindingType.ExpressionsAndFunctions);
			result = new CompilerResult<IInputStatement>(bind.Result, bind.ErrorOcured);
		}

		public void Visit(Ast.EmptyStatement emptyStat) {
			result = CompilerResult<IInputStatement>.Error;
		}

		#endregion

	}
}

using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	class InputCompilerVisitor : Ast.IInputVisitor {


		private InputCompiler inCompiler;


		private CompilerResult<IInputStatement> result;


		public InputCompilerVisitor(InputCompiler inComp) {
			inCompiler = inComp;
		}


		public CompilerResult<IInputStatement> TryCompile(Ast.IInputStatement astStatement) {

			astStatement.Accept(this);
			return result;
		}



		#region IInputVisitor Members

		public void Visit(Ast.ConstantDefinition constDef) {
			result = inCompiler.CompileConstDef(constDef);
		}

		public void Visit(Ast.EmptyStatement emptyStat) {
			result = CompilerResult<IInputStatement>.Error;
		}

		public void Visit(Ast.FunctionDefinition funDef) {
			result = inCompiler.CompileFunctionDef(funDef);
		}

		public void Visit(Ast.LsystemDefinition lsysDef) {
			var prms = inCompiler.CompileParameters(lsysDef.Parameters);
			var stats = inCompiler.LsystemCompiler.CompileLsystemStatements(lsysDef.Statements);

			result = new Lsystem(lsysDef.NameId.Name, prms, stats, lsysDef);
		}

		#endregion
	}
}

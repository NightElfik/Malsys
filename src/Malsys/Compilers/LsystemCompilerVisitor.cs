using System;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	class LsystemCompilerVisitor : Ast.ILsystemVisitor {

		private InputCompiler inCompiler;


		private CompilerResult<ILsystemStatement> result;


		public LsystemCompilerVisitor(InputCompiler inComp) {
			inCompiler = inComp;
		}


		public CompilerResult<ILsystemStatement> TryCompile(Ast.ILsystemStatement astStatement) {
			astStatement.Accept(this);
			return result;
		}


		#region ILsystemVisitor Members

		public void Visit(Ast.ConstantDefinition constDef) {
			result = inCompiler.CompileConstDef(constDef);
		}

		public void Visit(Ast.EmptyStatement emptyStat) {
			result = CompilerResult<ILsystemStatement>.Error;
		}

		public void Visit(Ast.FunctionDefinition funDef) {
			result = inCompiler.CompileFunctionDef(funDef);
		}

		public void Visit(Ast.RewriteRule rewriteRule) {
			result = inCompiler.LsystemCompiler.CompileRewriteRule(rewriteRule);
		}

		public void Visit(Ast.SymbolsInterpretDef symbolInterpretDef) {
			result = inCompiler.LsystemCompiler.CompileSymbolsInterpretation(symbolInterpretDef);
		}

		public void Visit(Ast.SymbolsConstDefinition symbolsDef) {
			result = inCompiler.LsystemCompiler.CompileSymbolConstant(symbolsDef);
		}

		#endregion
	}
}

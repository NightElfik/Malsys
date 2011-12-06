using System;
using System.Collections.Generic;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	internal class InputCompiler : IInputCompiler, Ast.IInputVisitor {

		private IConstantDefinitionCompiler constDefCompiler;
		private IFunctionDefinitionCompiler funDefCompiler;
		private ILsystemCompiler lsysCompiler;
		private IExpressionCompiler exprCompiler;


		private CompilerResult<IInputStatement> visitResult;


		public InputCompiler(IConstantDefinitionCompiler constantDefCompiler, IFunctionDefinitionCompiler functionDefCompiler,
				ILsystemCompiler lsystemCompiler, IExpressionCompiler expressionCompiler) {

			constDefCompiler = constantDefCompiler;
			funDefCompiler = functionDefCompiler;
			lsysCompiler = lsystemCompiler;
			exprCompiler = expressionCompiler;
		}



		public InputBlock Compile(Ast.InputBlock parsedInput) {

			var statements = new List<IInputStatement>(parsedInput.Statements.Length);

			for (int i = 0; i < parsedInput.Statements.Length; i++) {
				parsedInput.Statements[i].Accept(this);
				if (visitResult) {
					statements.Add(visitResult.Result);
				}
			}

			var statsImm = new ImmutableList<IInputStatement>(statements);
			return new InputBlock(parsedInput.SourceName, statsImm);
		}


		#region IInputVisitor Members

		public void Visit(Ast.ConstantDefinition constDef) {
			visitResult = constDefCompiler.Compile(constDef);
		}

		public void Visit(Ast.EmptyStatement emptyStat) {
			visitResult = CompilerResult<IInputStatement>.Error;
		}

		public void Visit(Ast.FunctionDefinition funDef) {
			visitResult = funDefCompiler.Compile(funDef);
		}

		public void Visit(Ast.LsystemDefinition lsysDef) {
			visitResult = lsysCompiler.Compile(lsysDef);
		}

		public void Visit(Ast.ProcessConfigurationDefinition processConfDef) {
			throw new NotImplementedException();
		}

		public void Visit(Ast.ProcessStatement processDef) {
			throw new NotImplementedException();
		}

		#endregion
	}

}

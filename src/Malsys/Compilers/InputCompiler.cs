using System;
using System.Collections.Generic;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	internal class InputCompiler : IInputCompiler, Ast.IInputVisitor {

		private IConstantDefinitionCompiler constDefCompiler;
		private IFunctionDefinitionCompiler funDefCompiler;
		private ILsystemCompiler lsysCompiler;
		private IExpressionCompiler exprCompiler;

		private IMessageLogger logger;
		private CompilerResult<IInputStatement> visitResult;


		public InputCompiler(IConstantDefinitionCompiler constantDefCompiler, IFunctionDefinitionCompiler functionDefCompiler,
				ILsystemCompiler lsystemCompiler, IExpressionCompiler expressionCompiler) {

			constDefCompiler = constantDefCompiler;
			funDefCompiler = functionDefCompiler;
			lsysCompiler = lsystemCompiler;
			exprCompiler = expressionCompiler;
		}



		public InputBlock Compile(Ast.InputBlock parsedInput, IMessageLogger logger) {

			var statements = new List<IInputStatement>(parsedInput.Statements.Length);
			this.logger = logger;

			for (int i = 0; i < parsedInput.Statements.Length; i++) {
				parsedInput.Statements[i].Accept(this);
				if (visitResult) {
					statements.Add(visitResult.Result);
				}
			}

			logger = null;
			var statsImm = new ImmutableList<IInputStatement>(statements);
			return new InputBlock(parsedInput.SourceName, statsImm);
		}


		#region IInputVisitor Members

		public void Visit(Ast.ConstantDefinition constDef) {
			visitResult = constDefCompiler.Compile(constDef, logger);
		}

		public void Visit(Ast.EmptyStatement emptyStat) {
			visitResult = CompilerResult<IInputStatement>.Error;
		}

		public void Visit(Ast.FunctionDefinition funDef) {
			visitResult = funDefCompiler.Compile(funDef, logger);
		}

		public void Visit(Ast.LsystemDefinition lsysDef) {
			visitResult = lsysCompiler.Compile(lsysDef, logger);
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

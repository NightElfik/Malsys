using System;
using System.Collections.Generic;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	internal class InputCompiler : IInputCompiler, Ast.IInputVisitor {

		private IConstantDefinitionCompiler constDefCompiler;
		private IFunctionDefinitionCompiler funDefCompiler;
		private ILsystemCompiler lsysCompiler;
		private IExpressionCompiler exprCompiler;
		private IProcessStatementsCompiler processStatsCompiler;

		private IMessageLogger logger;
		private CompilerResult<IInputStatement> visitResult;


		public InputCompiler(IConstantDefinitionCompiler constantDefCompiler, IFunctionDefinitionCompiler functionDefCompiler,
				ILsystemCompiler lsystemCompiler, IExpressionCompiler expressionCompiler, IProcessStatementsCompiler processStatementsCompiler) {

			constDefCompiler = constantDefCompiler;
			funDefCompiler = functionDefCompiler;
			lsysCompiler = lsystemCompiler;
			exprCompiler = expressionCompiler;
			processStatsCompiler = processStatementsCompiler;
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
			return new InputBlock(parsedInput.SourceName, statements.ToImmutableList());
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
			visitResult = processStatsCompiler.Compile(processConfDef, logger);
		}

		public void Visit(Ast.ProcessStatement processStat) {
			visitResult = processStatsCompiler.Compile(processStat, logger);
		}

		#endregion
	}

}

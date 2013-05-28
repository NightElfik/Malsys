// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;
using System.Diagnostics;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	/// <summary>
	/// The main compiler for compilation of the AST.
	/// </summary>
	/// <remarks>
	/// All public members are thread safe if supplied compilers are thread safe.
	/// </remarks>
	public class InputCompiler : IInputCompiler {

		protected readonly IConstantDefinitionCompiler constDefCompiler;
		protected readonly IFunctionDefinitionCompiler funDefCompiler;
		protected readonly ILsystemCompiler lsysCompiler;
		protected readonly IExpressionCompiler exprCompiler;
		protected readonly IProcessStatementsCompiler processStatsCompiler;


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

			foreach (var stat in parsedInput.Statements) {
				switch (stat.StatementType) {

					case Ast.InputStatementType.EmptyStatement:
						break;

					case Ast.InputStatementType.ConstantDefinition:
						statements.Add(constDefCompiler.Compile((Ast.ConstantDefinition)stat, logger));
						break;

					case Ast.InputStatementType.FunctionDefinition:
						statements.Add(funDefCompiler.Compile((Ast.FunctionDefinition)stat, logger));
						break;

					case Ast.InputStatementType.LsystemDefinition:
						statements.Add(lsysCompiler.Compile((Ast.LsystemDefinition)stat, logger));
						break;

					case Ast.InputStatementType.ProcessStatement:
						statements.Add(processStatsCompiler.Compile((Ast.ProcessStatement)stat, logger));
						break;

					case Ast.InputStatementType.ProcessConfigurationDefinition:
						statements.Add(processStatsCompiler.Compile((Ast.ProcessConfigurationDefinition)stat, logger));
						break;

					default:
						Debug.Fail("Unknown input statement type `{0}`.".Fmt(stat.StatementType.ToString()));
						break;

				}
			}

			return new InputBlock(parsedInput.SourceName, statements.ToImmutableList());
		}

	}

}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Malsys.Parsing;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.FSharp.Core;

namespace Malsys.Compilers {
	public static class InputCompiler {
		public static CompilerResult CompileFromString(string strInput, string sourceName, CompilerParameters prms) {
			var lexBuff = LexBuffer<char>.FromString(strInput);
			return compile(lexBuff, sourceName, prms);
		}

		private static CompilerResult compile(LexBuffer<char> lexBuff, string sourceName, CompilerParameters prms) {

			bool wasError = false;
			var cp = new CompilerParametersInternal(prms);
			cp.Messages.DefaultSourceName = sourceName;

			Ast.IInputStatement[] parsedStmnts = new Ast.IInputStatement[0];

			parsedStmnts = ParserUtils.parseLsystemStatements(lexBuff, cp.Messages, sourceName);
			if (cp.Messages.ErrorOcured) {
				wasError = true;
			}

			List<LsystemDefinition> lsysDefs = new List<LsystemDefinition>();
			List<VariableDefinition> varDefs = new List<VariableDefinition>();
			List<FunctionDefinition> funDefs = new List<FunctionDefinition>();

			foreach (var statement in parsedStmnts) {

				if (statement is Malsys.Ast.Lsystem) {
					LsystemDefinition lsys;
					if (LsystemCompiler.TryCompile((Ast.Lsystem)statement, cp, out lsys)) {
						lsysDefs.Add(lsys);
					}
					else {
						wasError = true;
					}
				}

				else if (statement is Malsys.Ast.VariableDefinition) {
					VariableDefinition varDef;
					if (VariableDefinitionCompiler.TryCompile((Ast.VariableDefinition)statement, cp, out varDef)) {
						varDefs.Add(varDef);
					}
					else {
						wasError = true;
					}
				}

				else if (statement is Malsys.Ast.FunctionDefinition) {
					FunctionDefinition funDef;
					if (FunctionDefinitionCompiler.TryCompile((Ast.FunctionDefinition)statement, cp, out funDef)) {
						funDefs.Add(funDef);
					}
					else {
						wasError = true;
					}
				}

				else {
					Debug.Fail("Unhandled type `{0}` of {1} while compiling input block `{2}`.".Fmt(
						statement.GetType().Name, typeof(Ast.IInputStatement).Name, sourceName));

					cp.Messages.AddMessage("Internal compiler error.", CompilerMessageType.Error, statement.Position);
					wasError = true;
				}
			}


			if (wasError) {
				cp.Messages.AddMessage("Some error ocured during compilation of `{0}`".Fmt(sourceName), CompilerMessageType.Error, Position.Unknown);
				Debug.Assert(cp.Messages.ErrorOcured, "Compiler's result error state is false, but error ocured.");
				return new CompilerResult(cp.Messages, null);
			}

			var lsysImm = new ImmutableList<LsystemDefinition>(lsysDefs);
			var varsImm = new ImmutableList<VariableDefinition>(varDefs);
			var funsImm = new ImmutableList<FunctionDefinition>(funDefs);
			var inputBlock = new InputBlock(lsysImm, varsImm, funsImm);

			Debug.Assert(cp.Messages.ErrorOcured, "Compiler's result error state is true, but no error ocured.");

			return new CompilerResult(cp.Messages, inputBlock);
		}
	}
}

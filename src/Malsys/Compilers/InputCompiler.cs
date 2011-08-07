using System;
using System.Collections.Generic;
using System.Diagnostics;
using Malsys.Parsing;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.FSharp.Core;

namespace Malsys.Compilers {
	public static class InputCompiler {

		public static CompilerResult<InputBlock> CompileFromString(string strInput, string sourceName, MessagesCollection msgs) {

			var lexBuff = LexBuffer<char>.FromString(strInput);
			msgs.DefaultSourceName = sourceName;
			var comments = new List<Ast.Comment>();

			var parsedInput = ParserUtils.parseLsystemStatements(lexBuff, comments, msgs, sourceName);

			return CompileFromAst(parsedInput, msgs);
		}

		public static CompilerResult<InputBlock> CompileFromAst(Ast.InputBlock parsedInput, MessagesCollection msgs) {

			bool wasError = msgs.ErrorOcured;

			List<LsystemDefinition> lsysDefs = new List<LsystemDefinition>();
			List<VariableDefinition> varDefs = new List<VariableDefinition>();
			List<FunctionDefinition> funDefs = new List<FunctionDefinition>();

			foreach (var statement in parsedInput) {

				if (statement is Malsys.Ast.Lsystem) {
					var lsysResult = ((Ast.Lsystem)statement).Compile(msgs);
					if (lsysResult) {
						lsysDefs.Add(lsysResult);
					}
				}

				else if (statement is Ast.VariableDefinition) {
					var vd = ((Ast.VariableDefinition)statement).CompileFailSafe(msgs);
					varDefs.Add(vd);
				}

				else if (statement is Ast.FunctionDefinition) {
					var fd = ((Ast.FunctionDefinition)statement).CompileFailSafe(msgs);
					funDefs.Add(fd);
				}

				else if (statement is Ast.EmptyStatement) {
					msgs.AddMessage("Empty statement found.", CompilerMessageType.Notice, statement.Position);
				}

				else {
					Debug.Fail("Unhandled type `{0}` of {1} while compiling input block `{2}`.".Fmt(
						statement.GetType().Name, typeof(Ast.IInputStatement).Name, msgs.DefaultSourceName));

					msgs.AddError("Internal L-system input compiler error.", statement.Position);
					wasError = true;
				}
			}


			if (wasError) {
				msgs.AddError("Some error ocured during compilation of `{0}`.".Fmt(msgs.DefaultSourceName), Position.Unknown);
				Debug.Assert(msgs.ErrorOcured, "Compiler's result error state is false, but error ocured.");
				return CompilerResult<InputBlock>.Error;
			}

			var lsysImm = new ImmutableList<LsystemDefinition>(lsysDefs);
			var varsImm = new ImmutableList<VariableDefinition>(varDefs);
			var funsImm = new ImmutableList<FunctionDefinition>(funDefs);

			Debug.Assert(msgs.ErrorOcured, "Compiler's result error state is true, but no error ocured.");

			return new InputBlock(lsysImm, varsImm, funsImm);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Malsys.Parsing;
using Microsoft.FSharp.Text.Lexing;

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

			try {
				parsedStmnts = ParserUtils.parseLsystemStatements(lexBuff, sourceName);
			}
			catch (LexerException ex) {
				wasError = true;
				cp.Messages.AddMessage("Failed to parse (lex) input. " + ex.LexerMessage, CompilerMessageType.Error, new Ast.Position(ex.BeginPosition, ex.EndPosition));
			}
			catch (ParserException<char> ex) {
				wasError = true;
				cp.Messages.AddMessage("Failed to parse input. " + ex.ParserMessage, CompilerMessageType.Error, new Ast.Position(ex.ErrorContext.ParseState.ResultRange));
			}
			catch (Exception ex) {
				wasError = true;
				cp.Messages.AddMessage("{0} occured while parsing input `{1}`. ".Fmt(ex.GetType().Name, sourceName),
					CompilerMessageType.Error, Ast.Position.Unknown);
			}


			List<LsystemDefinition> lsysDefs = new List<LsystemDefinition>();
			List<VariableDefinition> varDefs = new List<VariableDefinition>();
			List<FunctionDefinition> funDefs = new List<FunctionDefinition>();

			foreach (var statement in parsedStmnts) {
				try {
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
				catch (Exception ex) {
					cp.Messages.AddMessage("{0} occured while compiling input `{1}`. ".Fmt(ex.GetType().Name, sourceName),
						CompilerMessageType.Error, statement.Position);
				}
			}


			if (wasError) {
				cp.Messages.AddMessage("Some error ocured during compilation of `{0}`".Fmt(sourceName), CompilerMessageType.Error, Ast.Position.Unknown);
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

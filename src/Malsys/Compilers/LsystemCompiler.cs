using System.Collections.Generic;
using System.Diagnostics;
using Malsys.Expressions;

namespace Malsys.Compilers {
	public static class LsystemCompiler {
		/// <summary>
		/// Thread safe.
		/// </summary>
		public static CompilerResult<LsystemDefinition> Compile(this Ast.Lsystem lsysAst, MessagesCollection msgs) {

			var prms = FunctionDefinitionCompiler.CompileParametersFailSafe(lsysAst.Parameters, msgs);

			var rRules = new List<RewriteRule>();
			var varDefs = new List<VariableDefinition<IExpression>>();
			var symDefs = new List<VariableDefinition<SymbolsList<IExpression>>>();
			var funDefs = new List<FunctionDefinition>();

			foreach (var statement in lsysAst.Body) {

				if (statement is Ast.RewriteRule) {
					var rrResult = ((Ast.RewriteRule)statement).Compile(msgs);
					if (rrResult) {
						rRules.Add(rrResult);
					}
				}

				else if (statement is Ast.VariableDefinition) {
					var vd = ((Ast.VariableDefinition)statement).CompileFailSafe(msgs);
					varDefs.Add(vd);
				}

				else if (statement is Ast.SymbolsDefinition) {
					var sd = ((Ast.SymbolsDefinition)statement).CompileFailSafe(msgs);
					symDefs.Add(sd);
				}

				else if (statement is Ast.FunctionDefinition) {
					var fd = ((Ast.FunctionDefinition)statement).CompileFailSafe(msgs);
					funDefs.Add(fd);
				}

				else if (statement is Ast.EmptyStatement) {
					if (!((Ast.EmptyStatement)statement).Hidden) {
						msgs.AddMessage("Empty statement found.", CompilerMessageType.Notice, statement.Position);
					}
				}

				else {
					Debug.Fail("Unhandled type `{0}` of {1} while compiling L-system `{2}`".Fmt(
						statement.GetType().Name, typeof(Ast.ILsystemStatement).Name, lsysAst.NameId.Name));

					msgs.AddError("Internal L-system compiler error.", statement.Position);
				}

			}


			var rRulesImm = new ImmutableList<RewriteRule>(rRules);
			var varDefsImm = new ImmutableList<VariableDefinition<IExpression>>(varDefs);
			var symDefsImm = new ImmutableList<VariableDefinition<SymbolsList<IExpression>>>(symDefs);
			var funDefsImm = new ImmutableList<FunctionDefinition>(funDefs);
			return new LsystemDefinition(lsysAst.NameId.Name, prms, rRulesImm, varDefsImm, symDefsImm, funDefsImm);
		}
	}
}

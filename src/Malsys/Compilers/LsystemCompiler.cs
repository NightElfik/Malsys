using System.Collections.Generic;
using System.Diagnostics;
using Malsys.Expressions;

namespace Malsys.Compilers {
	public static class LsystemCompiler {
		/// <summary>
		/// Thread safe.
		/// </summary>
		public static CompilerResult<LsystemDefinition> Compile(Ast.Lsystem lsysAst, MessagesCollection msgs) {

			var paramsTuple = FunctionDefinitionCompiler.CompileParametersFailSafe(lsysAst.Parameters, msgs);

			List<RewriteRule> rRules = new List<RewriteRule>();
			List<VariableDefinition> varDefs = new List<VariableDefinition>();
			List<FunctionDefinition> funDefs = new List<FunctionDefinition>();

			foreach (var statement in lsysAst.Statements) {

				if (statement is Ast.RewriteRule) {
					var rrResult = RewriteRuleCompiler.Compile((Ast.RewriteRule)statement, msgs);
					if (rrResult) {
						rRules.Add(rrResult);
					}
				}

				else if (statement is Ast.VariableDefinition) {
					var vd = VariableDefinitionCompiler.CompileFailSafe((Ast.VariableDefinition)statement, msgs);
					varDefs.Add(vd);
				}

				else if (statement is Ast.FunctionDefinition) {
					var fd = FunctionDefinitionCompiler.CompileFailSafe((Ast.FunctionDefinition)statement, msgs);
					funDefs.Add(fd);
				}

				else if (statement is Ast.EmptyStatement) {
					msgs.AddMessage("Empty statement found.", CompilerMessageType.Notice, statement.Position);
				}

				else {
					Debug.Fail("Unhandled type `{0}` of {1} while compiling L-system `{2}`".Fmt(
						statement.GetType().Name, typeof(Ast.ILsystemStatement).Name, lsysAst.NameId.Name));

					msgs.AddError("Internal L-system compiler error.", statement.Position);
				}

			}


			var rRulesImm = new ImmutableList<RewriteRule>(rRules);
			var varDefsImm = new ImmutableList<VariableDefinition>(varDefs);
			var funDefsImm = new ImmutableList<FunctionDefinition>(funDefs);
			return new LsystemDefinition(lsysAst.NameId.Name, paramsTuple.Item1, paramsTuple.Item2, funDefsImm, varDefsImm, rRulesImm);
		}
	}
}

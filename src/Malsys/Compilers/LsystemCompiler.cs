using System.Collections.Generic;
using System.Diagnostics;
using Malsys.Expressions;

namespace Malsys.Compilers {

	public static class LsystemCompiler {
		/// <summary>
		/// Thread safe.
		/// </summary>
		public static bool TryCompile(Ast.Lsystem lsysAst, CompilerParameters prms, out LsystemDefinition result) {

			if (tryCompile(lsysAst, prms, out result)) {
				prms.Messages.AddMessage("Failed to compile L-system definition `{0}`.".Fmt(lsysAst.NameId.Name),
					CompilerMessageType.Error, lsysAst.Position);
				result = null;
				return false;
			}

			return true;
		}


		private static bool tryCompile(Ast.Lsystem lsysAst, CompilerParameters prms, out LsystemDefinition result) {

			ImmutableList<string> paramsNames;
			ImmutableList<IValue> optParamsValues;
			if (!FunctionDefinitionCompiler.TryCompileParameters(lsysAst.Parameters, prms, out paramsNames, out optParamsValues)) {
				result = null;
				return false;
			}

			List<RewriteRule> rRules = new List<RewriteRule>();
			List<VariableDefinition> varDefs = new List<VariableDefinition>();
			List<FunctionDefinition> funDefs = new List<FunctionDefinition>();

			foreach (var stmt in lsysAst.Statements) {

				if (stmt is Ast.RewriteRule) {
					RewriteRule rr;
					if (!RewriteRuleCompiler.TryCompile((Ast.RewriteRule)stmt, prms, out rr)) {
						result = null;
						return false;
					}
					rRules.Add(rr);
				}

				else if (stmt is Ast.VariableDefinition) {
					VariableDefinition vd;
					if (!VariableDefinitionCompiler.TryCompile((Ast.VariableDefinition)stmt, prms, out vd)) {
						result = null;
						return false;
					}
					varDefs.Add(vd);
				}

				else if (stmt is Ast.FunctionDefinition) {
					FunctionDefinition fd;
					if (!FunctionDefinitionCompiler.TryCompile((Ast.FunctionDefinition)stmt, prms, out fd)) {
						result = null;
						return false;
					}
					funDefs.Add(fd);
				}
				else {
					Debug.Fail("Unhandled type `{0}` of {1} while compiling L-system `{0}`".Fmt(
						stmt.GetType().Name, typeof(Ast.ILsystemStatement).Name), lsysAst.NameId.Name);

					prms.Messages.AddMessage("Internal compiler error.".Fmt(lsysAst.NameId.Name),
						CompilerMessageType.Error, stmt.Position);
					result = null;
					return false;
				}
			}


			string name = prms.CaseSensitiveLsystemNames ? lsysAst.NameId.Name : lsysAst.NameId.Name.ToLowerInvariant();
			var rRulesImm = new ImmutableList<RewriteRule>(rRules);
			var varDefsImm = new ImmutableList<VariableDefinition>(varDefs);
			var funDefsImm = new ImmutableList<FunctionDefinition>(funDefs);

			result = new LsystemDefinition(name, paramsNames, optParamsValues, funDefsImm, varDefsImm, rRulesImm);
			return true;
		}

	}
}

using System.Collections.Generic;
using Malsys.Expressions;

namespace Malsys.Compilers {
	public static class RewriteRuleCompiler {
		/// <summary>
		/// Thread safe.
		/// </summary>
		public static CompilerResult<RewriteRule> Compile(Ast.RewriteRule rRuleAst, MessagesCollection msgs) {

			var usedNames = new Dictionary<string, Position>();

			var ptrn = SymbolsCompiler.Compile(rRuleAst.Pattern, usedNames, msgs);
			if (!ptrn) {
				return CompilerResult<RewriteRule>.Error;
			}

			var lCtxt = SymbolsCompiler.CompileListFailSafe(rRuleAst.LeftContext, usedNames, msgs);

			var rCtxt = SymbolsCompiler.CompileListFailSafe(rRuleAst.RightContext, usedNames, msgs);

			usedNames = null;


			var cond = ExpressionCompiler.CompileRichFailSafe(rRuleAst.Condition, msgs);

			var probab = ExpressionCompiler.CompileRichFailSafe(rRuleAst.Probability, msgs);

			var vars = VariableDefinitionCompiler.CompileFailSafe(rRuleAst.VariableDefs, msgs);

			var replac = SymbolsCompiler.CompileListFailSafe(rRuleAst.Replacement, msgs);


			return new RewriteRule(ptrn, lCtxt, rCtxt, cond, probab, vars, replac);
		}

	}
}

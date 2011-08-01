using System.Collections.Generic;
using Malsys.Expressions;

namespace Malsys.Compilers {
	public static class RewriteRuleCompiler {
		/// <summary>
		/// Thread safe.
		/// </summary>
		public static CompilerResult<RewriteRule> Compile(this Ast.RewriteRule rRuleAst, MessagesCollection msgs) {

			var usedNames = new Dictionary<string, Position>();

			var ptrn = rRuleAst.Pattern.Compile(usedNames, msgs);
			if (!ptrn) {
				return CompilerResult<RewriteRule>.Error;
			}

			var lCtxt = rRuleAst.LeftContext.CompileListFailSafe(usedNames, msgs);

			var rCtxt = rRuleAst.RightContext.CompileListFailSafe(usedNames, msgs);

			usedNames = null;


			var cond = rRuleAst.Condition.IsEmpty
				? RichExpression.True
				: rRuleAst.Condition.CompileRichFailSafe(msgs);

			var probab = rRuleAst.Probability.IsEmpty
				? RichExpression.One
				: rRuleAst.Probability.CompileRichFailSafe(msgs);

			var vars = rRuleAst.VariableDefs.CompileFailSafe(msgs);

			var replac = rRuleAst.Replacement.CompileListFailSafe(msgs);


			return new RewriteRule(ptrn, lCtxt, rCtxt, cond, probab, vars, replac);
		}

	}
}

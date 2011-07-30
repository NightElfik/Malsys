using System.Collections.Generic;
using Malsys.Expressions;

namespace Malsys.Compilers {
	public static class RewriteRuleCompiler {
		/// <summary>
		/// Thread safe.
		/// </summary>
		public static bool TryCompile(Ast.RewriteRule rRuleAst, CompilerParametersInternal prms, out RewriteRule result) {

			if (!tryCompile(rRuleAst, prms, out result)) {
				prms.Messages.AddMessage("Failed to compile rewrite rule.", CompilerMessageType.Error, rRuleAst.Position);
				result = null;
				return false;
			}

			return true;
		}


		private static bool tryCompile(Ast.RewriteRule rRuleAst, CompilerParametersInternal prms, out RewriteRule result) {

			var usedNames = new Dictionary<string, Position>();

			Symbol<string> ptrn;
			if (!SymbolsCompiler.TryCompile(rRuleAst.Pattern, usedNames, prms, out ptrn)) {
				prms.Messages.AddMessage("Failed to compile pattern.", CompilerMessageType.Error, rRuleAst.Pattern.Position);
				result = null;
				return false;
			}

			SymbolsList<string> lCtxt;
			if(!SymbolsCompiler.TryCompile(rRuleAst.LeftContext, usedNames, prms, out lCtxt)){
				prms.Messages.AddMessage("Failed to compile left context.", CompilerMessageType.Error, rRuleAst.LeftContext.Position);
				result = null;
				return false;
			}

			SymbolsList<string> rCtxt;
			if(!SymbolsCompiler.TryCompile(rRuleAst.RightContext, usedNames, prms, out rCtxt)){
				prms.Messages.AddMessage("Failed to compile right context.", CompilerMessageType.Error, rRuleAst.RightContext.Position);
				result = null;
				return false;
			}

			usedNames = null;


			RichExpression cond;
			if (!ExpressionCompiler.TryCompileRich(rRuleAst.Condition, prms, out cond)) {
				prms.Messages.AddMessage("Failed to compile condition.", CompilerMessageType.Error, rRuleAst.Condition.Position);
				result = null;
				return false;
			}

			RichExpression probab;
			if (!ExpressionCompiler.TryCompileRich(rRuleAst.Probability, prms, out probab)) {
				prms.Messages.AddMessage("Failed to compile probability.", CompilerMessageType.Error, rRuleAst.Probability.Position);
				result = null;
				return false;
			}

			ImmutableList<VariableDefinition> vars;
			if(!VariableDefinitionCompiler.TryCompile(rRuleAst.VariableDefs, prms, out vars)){
				result = null;
				return false;
			}

			SymbolsList<IExpression> replac;
			if(!SymbolsCompiler.TryCompile(rRuleAst.Replacement, prms, out replac)){
				result = null;
				return false;
			}


			result = new RewriteRule(ptrn, lCtxt, rCtxt, cond, probab, vars, replac);
			return true;
		}

	}
}

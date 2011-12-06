using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.SemanticModel;
using PatternCompiler = Malsys.Compilers.ICompiler<Malsys.Ast.LsystemSymbol, Malsys.SemanticModel.Symbol<string>>;
using ReplacementCompiler = Malsys.Compilers.ICompiler<Malsys.Ast.LsystemSymbol, Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Compiled.IExpression>>;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	internal class RewriteRuleCompiler : IRewriteRuleCompiler {

		private MessageLogger msgs;
		private IConstantDefinitionCompiler constDefCompiler;
		private IExpressionCompiler exprCompiler;

		// to be able separate the interfaces on ISymbolCompiler
		private PatternCompiler patternCompiler;
		private ReplacementCompiler symbolCompiler;


		public RewriteRuleCompiler(MessageLogger messageLogger, IConstantDefinitionCompiler constantDefCompiler, ISymbolCompiler iSymbolCompiler,
				IExpressionCompiler expressionCompiler) {

			msgs = messageLogger;
			constDefCompiler = constantDefCompiler;
			patternCompiler = iSymbolCompiler;
			symbolCompiler = iSymbolCompiler;
		}


		public RewriteRule Compile(Ast.RewriteRule rRuleAst) {

			Symbol<string> ptrn = patternCompiler.Compile(rRuleAst.Pattern);
			var lCtxt = patternCompiler.CompileList(rRuleAst.LeftContext);
			var rCtxt = patternCompiler.CompileList(rRuleAst.RightContext);

			var usedNames = new Dictionary<string, Position>();
			checkPatternParams(lCtxt, usedNames);
			checkPatternParams(ptrn, usedNames);
			checkPatternParams(rCtxt, usedNames);
			usedNames = null;

			var locConsts = new ConstantDefinition[rRuleAst.LocalConstDefs.Length];
			for (int i = 0; i < rRuleAst.LocalConstDefs.Length; i++) {
				locConsts[i] = constDefCompiler.Compile(rRuleAst.LocalConstDefs[i]);
			}
			var locConstDefs = new ImmutableList<ConstantDefinition>(locConsts, true);

			var cond = rRuleAst.Condition.IsEmpty
				? Constant.True
				: exprCompiler.Compile(rRuleAst.Condition);

			var replacs = CompileReplacementsList(rRuleAst.Replacements);

			return new RewriteRule(ptrn, new SymbolsList<string>(lCtxt), new SymbolsList<string>(rCtxt), locConstDefs, cond, replacs);
		}

		private bool checkPatternParams(Symbol<string> symbol, Dictionary<string, Position> usedNames) {

			bool allAreUnique = true;

			for (int i = 0; i < symbol.Arguments.Length; i++) {
				string name = symbol.Arguments[i];
				if (name == "_") {
					continue;
				}

				if (usedNames.ContainsKey(name)) {
					var otherPos = usedNames[name];
					msgs.LogMessage(Message.PatternsParamNameNotUnique, symbol.AstSymbol.Arguments[i].Position, name, symbol.Name, otherPos);
					allAreUnique = false;
				}

				usedNames.Add(name, symbol.AstSymbol.Arguments[i].Position);
			}

			return allAreUnique;
		}

		private bool checkPatternParams(ImmutableList<Symbol<string>> symbolsList, Dictionary<string, Position> usedNames) {

			bool allAreUnique = true;

			foreach (var symbol in symbolsList) {
				allAreUnique &= checkPatternParams(symbol, usedNames);
			}

			return allAreUnique;
		}


		private RewriteRuleReplacement CompileReplacement(Ast.RewriteRuleReplacement replacAst) {

			var probab = replacAst.Weight.IsEmpty
				? Constant.One
				: exprCompiler.Compile(replacAst.Weight);

			var replac = symbolCompiler.CompileList(replacAst.Replacement);

			return new RewriteRuleReplacement(replac, probab);
		}

		private ImmutableList<RewriteRuleReplacement> CompileReplacementsList(ImmutableList<Ast.RewriteRuleReplacement> replacList) {

			var replac = new RewriteRuleReplacement[replacList.Length];

			for (int i = 0; i < replacList.Length; i++) {
				replac[i] = CompileReplacement(replacList[i]);
			}

			return new ImmutableList<RewriteRuleReplacement>(replac, true);
		}

		public enum Message {
			[Message(MessageType.Error, "Parameter name `{0}` in pattern `{1}` is not unique in its context. See also {2}.")]
			PatternsParamNameNotUnique,
		}

	}
}

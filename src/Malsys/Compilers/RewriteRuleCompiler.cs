/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Collections.Generic;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using PatternCompiler = Malsys.Compilers.ICompiler<Malsys.Ast.LsystemSymbol, Malsys.SemanticModel.Symbol<string>>;
using ReplacementCompiler = Malsys.Compilers.ICompiler<Malsys.Ast.LsystemSymbol, Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Compiled.IExpression>>;

namespace Malsys.Compilers {
	/// <remarks>
	/// All public members are thread safe if supplied compilers are thread safe.
	/// </remarks>
	public class RewriteRuleCompiler : IRewriteRuleCompiler {

		protected readonly IConstantDefinitionCompiler constDefCompiler;
		protected readonly IExpressionCompiler exprCompiler;

		// to be able separate the interfaces on ISymbolCompiler
		protected readonly PatternCompiler patternCompiler;
		protected readonly ReplacementCompiler symbolCompiler;


		public RewriteRuleCompiler(IConstantDefinitionCompiler constantDefCompiler, ISymbolCompiler iSymbolCompiler,
				IExpressionCompiler expressionCompiler) {

			constDefCompiler = constantDefCompiler;
			patternCompiler = iSymbolCompiler;
			symbolCompiler = iSymbolCompiler;
			exprCompiler = expressionCompiler;
		}


		public RewriteRule Compile(Ast.RewriteRule rRuleAst, IMessageLogger logger) {

			Symbol<string> ptrn = patternCompiler.Compile(rRuleAst.Pattern, logger);
			var lCtxt = patternCompiler.CompileList(rRuleAst.LeftContext, logger);
			var rCtxt = patternCompiler.CompileList(rRuleAst.RightContext, logger);

			var usedNames = new Dictionary<string, Position>();
			checkPatternParams(lCtxt, usedNames, logger);
			checkPatternParams(ptrn, usedNames, logger);
			checkPatternParams(rCtxt, usedNames, logger);
			usedNames = null;

			var locConsts = new ConstantDefinition[rRuleAst.LocalConstDefs.Length];
			for (int i = 0; i < rRuleAst.LocalConstDefs.Length; i++) {
				locConsts[i] = constDefCompiler.Compile(rRuleAst.LocalConstDefs[i], logger);
			}
			var locConstDefs = new ImmutableList<ConstantDefinition>(locConsts, true);

			var cond = rRuleAst.Condition.IsEmpty
				? Constant.True
				: exprCompiler.Compile(rRuleAst.Condition, logger);

			var replacs = CompileReplacementsList(rRuleAst.Replacements, logger);

			return new RewriteRule(ptrn, new SymbolsList<string>(lCtxt), new SymbolsList<string>(rCtxt), locConstDefs, cond, replacs);
		}


		private bool checkPatternParams(Symbol<string> symbol, Dictionary<string, Position> usedNames, IMessageLogger logger) {

			bool allAreUnique = true;

			for (int i = 0; i < symbol.Arguments.Length; i++) {
				string name = symbol.Arguments[i];
				if (name == "_") {
					continue;
				}

				if (usedNames.ContainsKey(name)) {
					var otherPos = usedNames[name];
					logger.LogMessage(Message.PatternsParamNameNotUnique, symbol.AstSymbol.Arguments[i].Position, name, symbol.Name, otherPos);
					allAreUnique = false;
				}

				usedNames.Add(name, symbol.AstSymbol.Arguments[i].Position);
			}

			return allAreUnique;
		}

		private bool checkPatternParams(ImmutableList<Symbol<string>> symbolsList, Dictionary<string, Position> usedNames, IMessageLogger logger) {

			bool allAreUnique = true;

			foreach (var symbol in symbolsList) {
				allAreUnique &= checkPatternParams(symbol, usedNames, logger);
			}

			return allAreUnique;
		}


		private RewriteRuleReplacement CompileReplacement(Ast.RewriteRuleReplacement replacAst, IMessageLogger logger) {

			var probab = replacAst.Weight.IsEmpty
				? Constant.One
				: exprCompiler.Compile(replacAst.Weight, logger);

			var replac = symbolCompiler.CompileList(replacAst.Replacement, logger);

			return new RewriteRuleReplacement(replac, probab);
		}

		private ImmutableList<RewriteRuleReplacement> CompileReplacementsList(ImmutableList<Ast.RewriteRuleReplacement> replacList, IMessageLogger logger) {

			var replac = new RewriteRuleReplacement[replacList.Length];

			for (int i = 0; i < replacList.Length; i++) {
				replac[i] = CompileReplacement(replacList[i], logger);
			}

			return new ImmutableList<RewriteRuleReplacement>(replac, true);
		}


		public enum Message {

			[Message(MessageType.Error, "Parameter name `{0}` in pattern `{1}` is not unique in its context. See also {2}.")]
			PatternsParamNameNotUnique,

		}

	}
}

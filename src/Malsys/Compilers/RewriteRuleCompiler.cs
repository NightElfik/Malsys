using System.Collections.Generic;
using System.Linq;
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
			var rr = new RewriteRule(rRuleAst) {
				SymbolPattern = patternCompiler.Compile(rRuleAst.Pattern, logger),
				LeftContext = patternCompiler.CompileList(rRuleAst.LeftContext, logger),
				RightContext = patternCompiler.CompileList(rRuleAst.RightContext, logger),
				LocalConstantDefs = rRuleAst.LocalConstDefs.Select(lc => constDefCompiler.Compile(lc, logger)).ToList(),
				Condition = rRuleAst.Condition.IsEmpty
					? Constant.True
					: exprCompiler.Compile(rRuleAst.Condition, logger),
				Replacements = CompileReplacementsList(rRuleAst.Replacements, logger),
			};

			var usedNames = new Dictionary<string, PositionRange>();
			checkPatternParams(rr.LeftContext, usedNames, logger);
			checkPatternParams(rr.SymbolPattern, usedNames, logger);
			checkPatternParams(rr.RightContext, usedNames, logger);
			usedNames = null;

			return rr;
		}


		private bool checkPatternParams(Symbol<string> symbol, Dictionary<string, PositionRange> usedNames, IMessageLogger logger) {

			bool allAreUnique = true;

			for (int i = 0; i < symbol.Arguments.Count; i++) {
				string name = symbol.Arguments[i];
				if (name == "_") {
					continue;
				}

				var symbolAstNode = symbol.AstNode as Ast.LsystemSymbol;
				var symbolAstNodePos = symbolAstNode != null ? symbolAstNode.Arguments[i].Position : PositionRange.Unknown;

				if (usedNames.ContainsKey(name)) {
					var otherPos = usedNames[name];
					logger.LogMessage(Message.PatternsParamNameNotUnique,
						symbolAstNodePos, name, symbol.Name, otherPos);
					allAreUnique = false;
				}

				usedNames.Add(name, symbolAstNodePos);
			}

			return allAreUnique;
		}

		/// <summary>
		/// Checks if all pattern param names are unique.
		/// </summary>
		private bool checkPatternParams(List<Symbol<string>> symbolsList, Dictionary<string, PositionRange> usedNames, IMessageLogger logger) {
			return symbolsList.Aggregate(true, (acc, s) => acc & checkPatternParams(s, usedNames, logger));
		}


		private RewriteRuleReplacement CompileReplacement(Ast.RewriteRuleReplacement replacAst, IMessageLogger logger) {
			return new RewriteRuleReplacement(replacAst) {
				Replacement = symbolCompiler.CompileList(replacAst.Replacement, logger),
				Weight = replacAst.Weight.IsEmpty
					? Constant.One
					: exprCompiler.Compile(replacAst.Weight, logger),
			};
		}

		private List<RewriteRuleReplacement> CompileReplacementsList(IList<Ast.RewriteRuleReplacement> replacList, IMessageLogger logger) {
			return replacList.Select(r => CompileReplacement(r, logger)).ToList();
		}


		public enum Message {

			[Message(MessageType.Error, "Parameter name `{0}` in pattern `{1}` is not unique in its context. See also {2}.")]
			PatternsParamNameNotUnique,

		}

	}
}

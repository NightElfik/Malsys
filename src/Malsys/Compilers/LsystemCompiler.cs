using System.Collections.Generic;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;


namespace Malsys.Compilers {

	public class LsystemCompiler {
		/// <summary>
		/// Pattern that matches anything, but does not bind.
		/// </summary>
		public const string PatternPlaceholder = "_";


		private MessagesCollection msgs;

		private InputCompiler inputCompiler;
		private LsystemCompilerVisitor lsysStatementCompiler;


		public LsystemCompiler(InputCompiler iComp) {
			inputCompiler = iComp;
			msgs = inputCompiler.Messages;

			lsysStatementCompiler = new LsystemCompilerVisitor(inputCompiler);
		}


		public ImmutableList<ILsystemStatement> CompileLsystemStatements(ImmutableList<Ast.ILsystemStatement> statements) {

			var compStats = new List<ILsystemStatement>(statements.Count);

			foreach (var stat in statements) {

				var compiledStat = lsysStatementCompiler.TryCompile(stat);
				if (compiledStat) {
					compStats.Add((ILsystemStatement)compiledStat);
				}

			}

			return new ImmutableList<ILsystemStatement>(compStats);
		}

		public CompilerResult<RewriteRule> CompileRewriteRule(Ast.RewriteRule rRuleAst) {

			var ptrn = CompileSymbolAsPattern(rRuleAst.Pattern, true);
			var lCtxt = CompileSymbolsListAsPattern(rRuleAst.LeftContext, true);
			var rCtxt = CompileSymbolsListAsPattern(rRuleAst.RightContext, true);

			var usedNames = new Dictionary<string, Position>();
			CheckPatternParams(lCtxt, usedNames);
			CheckPatternParams(ptrn, usedNames);
			CheckPatternParams(rCtxt, usedNames);
			usedNames = null;

			var vars = inputCompiler.CompileBindingsList(rRuleAst.LocalBindings, BindingType.ExpressionsAndFunctions);

			var cond = rRuleAst.Condition.IsEmpty
				? Constant.True
				: inputCompiler.ExpressionCompiler.CompileExpression(rRuleAst.Condition);

			var replac = CompileReplacementsList(rRuleAst.Replacements);

			return new RewriteRule(ptrn, lCtxt, rCtxt, vars, cond, replac);
		}


		public RewriteRuleReplacement CompileReplacement(Ast.RewriteRuleReplacement replacAst) {

			var probab = replacAst.Weight.IsEmpty
				? Constant.One
				: inputCompiler.ExpressionCompiler.CompileExpression(replacAst.Weight);

			var replac = CompileSymbolsList(replacAst.Replacement);

			return new RewriteRuleReplacement(replac, probab);
		}

		public ImmutableList<RewriteRuleReplacement> CompileReplacementsList(ImmutableList<Ast.RewriteRuleReplacement> replacList) {

			var replac = new RewriteRuleReplacement[replacList.Length];

			for (int i = 0; i < replacList.Length; i++) {
				replac[i] = CompileReplacement(replacList[i]);
			}

			return new ImmutableList<RewriteRuleReplacement>(replac, true);
		}


		public bool CheckPatternParams(Symbol<string> symbol, Dictionary<string, Position> usedNames) {

			bool allAreUnique = true;

			for (int i = 0; i < symbol.Arguments.Length; i++) {
				string name = symbol.Arguments[i];
				if (name == PatternPlaceholder) {
					continue;
				}

				if (usedNames.ContainsKey(name)) {
					var otherPos = usedNames[name];
					msgs.AddError("Parameter name `{0}` in pattern `{1}` is not unique (in its context).".Fmt(name, symbol.Name),
						symbol.AstSymbol.Arguments[i].Position, otherPos);
					allAreUnique = false;
				}

				usedNames.Add(name, symbol.AstSymbol.Arguments[i].Position);
			}

			return allAreUnique;
		}

		public bool CheckPatternParams(SymbolsList<string> symbolsList, Dictionary<string, Position> usedNames) {

			bool allAreUnique = true;

			foreach (var symbol in symbolsList) {
				allAreUnique &= CheckPatternParams(symbol, usedNames);
			}

			return allAreUnique;
		}


		public Symbol<string> CompileSymbolAsPattern(Ast.LsystemSymbol symbol, bool allowArguments) {


			string[] names = null;

			if (allowArguments) {
				names = new string[symbol.Arguments.Length];
			}

			for (int i = 0; i < symbol.Arguments.Length; i++) {
				if (allowArguments) {
					if (symbol.Arguments[i].Members.Length != 1 || !(symbol.Arguments[i].Members[0] is Ast.Identificator)) {
						msgs.AddError("Argument of symbol `{0}` can be only one identificator in this context.".Fmt(symbol.Name), symbol.Arguments[i].Position);
					}

					names[i] = ((Ast.Identificator)symbol.Arguments[i].Members[0]).Name;
				}
				else {
					if (symbol.Arguments[i].Members.Length != 0) {
						msgs.AddError("Arguments of symbol `{0}` are not alowed in thos context.".Fmt(symbol.Name), symbol.Arguments[i].Position);
					}
				}
			}

			var namesImm = allowArguments ? new ImmutableList<string>(names, true) : ImmutableList<string>.Empty;
			return new Symbol<string>(symbol.Name, namesImm, symbol);
		}

		public SymbolsList<string> CompileSymbolsListAsPattern(Ast.ImmutableListPos<Ast.LsystemSymbol> symbolsList, bool allowArguments) {

			var compiledSymbols = new Symbol<string>[symbolsList.Length];

			for (int i = 0; i < symbolsList.Length; i++) {
				var symRslt = CompileSymbolAsPattern(symbolsList[i], allowArguments);
				compiledSymbols[i] = symRslt;
			}

			var compiledSymImm = new ImmutableList<Symbol<string>>(compiledSymbols, true);
			var result = new SymbolsList<string>(compiledSymImm);

			return result;
		}


		public Symbol<IExpression> CompileSymbol(Ast.LsystemSymbol symbol) {

			return new Symbol<IExpression>(symbol.Name, inputCompiler.ExpressionCompiler.CompileList(symbol.Arguments), symbol);
		}

		public SymbolsListExpr CompileSymbolsList(Ast.ImmutableListPos<Ast.LsystemSymbol> symbolsList) {

			var compiledSymbols = new Symbol<IExpression>[symbolsList.Length];

			for (int i = 0; i < symbolsList.Length; i++) {
				compiledSymbols[i] = CompileSymbol(symbolsList[i]);
			}

			return new SymbolsListExpr(new ImmutableList<Symbol<IExpression>>(compiledSymbols, true));
		}
	}
}

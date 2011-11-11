using System.Collections.Generic;
using System.Diagnostics;
using Malsys.Expressions;
using Microsoft.FSharp.Collections;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IExpression>;
using SymbolVarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SymbolsList<Malsys.Expressions.IExpression>>;


namespace Malsys.Compilers {

	public class LsystemCompiler {

		private MessagesCollection msgs;

		private InputCompiler inputCompiler;
		private ExpressionCompiler exprCompiler;


		public LsystemCompiler(InputCompiler iComp) {
			inputCompiler = iComp;
			exprCompiler = iComp.ExpressionCompiler;
			msgs = inputCompiler.Messages;
		}


		public CompilerResult<LsystemDefinition> Compile(Ast.Lsystem lsysAst) {

			var prms = inputCompiler.CompileParametersFailSafe(lsysAst.Parameters);

			var rRules = new List<RewriteRule>();
			var varDefs = MapModule.Empty<string, IExpression>();
			var symDefs = MapModule.Empty<string, SymbolsList<IExpression>>();
			var funDefs = MapModule.Empty<string, FunctionDefinition>();

			foreach (var statement in lsysAst.Body) {

				if (statement is Ast.RewriteRule) {
					var rrResult = Compile((Ast.RewriteRule)statement);
					if (rrResult) {
						rRules.Add(rrResult);
					}
				}

				else if (statement is Ast.VariableDefinition) {
					var vd = inputCompiler.CompileFailSafe((Ast.VariableDefinition)statement);
					bool oldWasSet;
					varDefs = varDefs.Add(vd.Name, vd.Value, out oldWasSet);
					if (oldWasSet) {
						// TODO: report
					}
				}

				else if (statement is Ast.SymbolsDefinition) {
					var sd = inputCompiler.CompileFailSafe((Ast.SymbolsDefinition)statement);
					bool oldWasSet;
					symDefs = symDefs.Add(sd.Name, sd.Value, out oldWasSet);
					if (oldWasSet) {
						// TODO: report
					}
				}

				else if (statement is Ast.FunctionDefinition) {
					var fd = inputCompiler.CompileFailSafe((Ast.FunctionDefinition)statement);
					bool oldWasSet;
					funDefs = funDefs.Add(fd.Name, fd, out oldWasSet);
					if (oldWasSet) {
						// TODO: report
					}
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
			return new LsystemDefinition(lsysAst.NameId.Name, prms, rRulesImm, varDefs, symDefs, funDefs);
		}

		public CompilerResult<RewriteRule> Compile(Ast.RewriteRule rRuleAst) {

			var usedNames = new Dictionary<string, Position>();

			var ptrn = Compile(rRuleAst.Pattern, usedNames);
			if (!ptrn) {
				return CompilerResult<RewriteRule>.Error;
			}

			var lCtxt = CompileListFailSafe(rRuleAst.LeftContext, usedNames);

			var rCtxt = CompileListFailSafe(rRuleAst.RightContext, usedNames);

			usedNames = null;

			var vars = inputCompiler.CompileFailSafe(rRuleAst.LocalVariables);

			var cond = rRuleAst.Condition.IsEmpty
				? Constant.True
				: exprCompiler.CompileExpression(rRuleAst.Condition);


			var replac = new List<RewriteRuleReplacement>();

			foreach (var r in rRuleAst.Replacements) {
				if (!r.Replacement.IsEmpty) {
					replac.Add(CompileReplacFailSafe(r));
				}
			}

			var replacImm = new ImmutableList<RewriteRuleReplacement>(replac);


			return new RewriteRule(ptrn, lCtxt, rCtxt, vars, cond, replacImm);
		}

		public RewriteRuleReplacement CompileReplacFailSafe(Ast.RewriteRuleReplacement replacAst) {
			var probab = replacAst.Weight.IsEmpty
				? Constant.One
				: exprCompiler.CompileExpression(replacAst.Weight);

			var replac = CompileListFailSafe(replacAst.Replacement);

			return new RewriteRuleReplacement(replac, probab);
		}

		public CompilerResult<Symbol<string>> Compile(Ast.Symbol<Ast.Identificator> ptrnAst, Dictionary<string, Position> usedNames) {

			var names = new string[ptrnAst.Arguments.Length];
			for (int i = 0; i < ptrnAst.Arguments.Length; i++) {
				string name = ptrnAst.Arguments[i].Name;
				names[i] = name;

				if (name == Constants.PatternPlaceholder) {
					continue;
				}

				if (usedNames.ContainsKey(name)) {
					var otherPos = usedNames[name];
					msgs.AddError("Parameter name `{0}` in pattern `{1}` is not unique (in its context).".Fmt(name, ptrnAst.Name),
						ptrnAst.Arguments[i].Position, otherPos);
					return CompilerResult<Symbol<string>>.Error;
				}

				usedNames.Add(name, ptrnAst.Arguments[i].Position);
			}

			var namesImm = new ImmutableList<string>(names, true);
			var result = new Symbol<string>(ptrnAst.Name, namesImm);

			return new CompilerResult<Symbol<string>>(result);
		}

		public SymbolsList<string> CompileListFailSafe(Ast.SymbolsListPos<Ast.Identificator> ptrnsAst, Dictionary<string, Position> usedNames) {

			var compiledSymbols = new Symbol<string>[ptrnsAst.Length];

			for (int i = 0; i < ptrnsAst.Length; i++) {
				var symRslt = Compile(ptrnsAst[i], usedNames);
				if (symRslt) {
					compiledSymbols[i] = symRslt;
				}
				else {
					return SymbolsList<string>.Empty;
				}
			}

			var compiledSymImm = new ImmutableList<Symbol<string>>(compiledSymbols, true);
			var result = new SymbolsList<string>(compiledSymImm);

			return result;
		}


		public Symbol<IExpression> CompileFailSafe(Ast.Symbol<Ast.Expression> symbolAst) {

			return new Symbol<IExpression>(symbolAst.Name, exprCompiler.CompileFailSafe(symbolAst.Arguments));

		}

		public SymbolsList<IExpression> CompileListFailSafe(Ast.SymbolsListPos<Ast.Expression> symbolsAst) {

			var compiledSymbols = new Symbol<IExpression>[symbolsAst.Length];

			for (int i = 0; i < symbolsAst.Length; i++) {
				compiledSymbols[i] = CompileFailSafe(symbolsAst[i]);
			}

			return new SymbolsList<IExpression>(new ImmutableList<Symbol<IExpression>>(compiledSymbols, true));
		}
	}
}

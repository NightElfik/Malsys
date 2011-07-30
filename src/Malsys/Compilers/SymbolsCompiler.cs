using System.Collections.Generic;
using Malsys.Expressions;

namespace Malsys.Compilers {
	public static class SymbolsCompiler {

		public static CompilerResult<Symbol<string>> Compile(Ast.SymbolPattern ptrnAst, Dictionary<string, Position> usedNames, MessagesCollection msgs) {

			var names = new string[ptrnAst.ParametersNames.Length];
			for (int i = 0; i < ptrnAst.ParametersNames.Length; i++) {
				string name = ptrnAst.ParametersNames[i].Name;

				if (usedNames.ContainsKey(name)) {
					var otherPos = usedNames[name];
					msgs.AddError("Parameter name `{0}` in pattern `{1}` is not unique (in its context).".Fmt(name, ptrnAst.Name),
						ptrnAst.ParametersNames[i].Position, otherPos);
					return CompilerResult<Symbol<string>>.Error;
				}

				usedNames.Add(name, ptrnAst.ParametersNames[i].Position);
				names[i] = name;
			}

			var namesImm = new ImmutableList<string>(names, true);
			var result = new Symbol<string>(ptrnAst.Name, namesImm);

			return new CompilerResult<Symbol<string>>(result);
		}

		public static SymbolsList<string> CompileListFailSafe(ImmutableList<Ast.SymbolPattern> ptrnsAst, Dictionary<string, Position> usedNames, MessagesCollection msgs) {

			var compiledSymbols = new Symbol<string>[ptrnsAst.Length];

			for (int i = 0; i < ptrnsAst.Length; i++) {
				var symRslt = Compile(ptrnsAst[i], usedNames, msgs);
				if (symRslt) {
					compiledSymbols[i] = symRslt;
				}
				else {
					return new CompilerResult<SymbolsList<string>>(SymbolsList<string>.Empty);
				}
			}

			var compiledSymImm = new ImmutableList<Symbol<string>>(compiledSymbols, true);
			var result = new SymbolsList<string>(compiledSymImm);

			return new CompilerResult<SymbolsList<string>>(result);
		}


		public static Symbol<IExpression> CompileFailSafe(Ast.SymbolExprArgs symbolAst, MessagesCollection msgs) {

			var args = ExpressionCompiler.CompileFailSafe(symbolAst.Arguments, msgs);
			return new Symbol<IExpression>(symbolAst.Name, args);
		}

		public static SymbolsList<IExpression> CompileListFailSafe(ImmutableList<Ast.SymbolExprArgs> symbolsAst, MessagesCollection msgs) {

			var compiledSymbols = new Symbol<IExpression>[symbolsAst.Length];

			for (int i = 0; i < symbolsAst.Length; i++) {
				compiledSymbols[i] = CompileFailSafe(symbolsAst[i], msgs);
			}

			return new SymbolsList<IExpression>(new ImmutableList<Symbol<IExpression>>(compiledSymbols, true));
		}

	}
}

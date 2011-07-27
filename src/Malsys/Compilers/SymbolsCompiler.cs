using System.Collections.Generic;
using Malsys.Expressions;

namespace Malsys.Compilers {
	public static class SymbolsCompiler {

		public static bool TryCompile(Ast.SymbolPattern ptrnAst, Dictionary<string, Ast.Position> usedNames, CompilerParameters prms, out Symbol<string> result) {

			var names = new string[ptrnAst.ParametersNames.Length];
			for (int i = 0; i < ptrnAst.ParametersNames.Length; i++) {
				string name = prms.CaseSensitiveVarsNames ? ptrnAst.ParametersNames[i].Name : ptrnAst.ParametersNames[i].Name.ToLowerInvariant();

				if (usedNames.ContainsKey(name)) {
					var otherPos = usedNames[name];
					prms.Messages.AddMessage("Parameter name `{0}` in pattern `{1}` is not unique. Another occurance is at line {2} col {3}.".Fmt(
							ptrnAst.ParametersNames[i].Name, ptrnAst.Name, otherPos.BeginLine, otherPos.BeginColumn),
						CompilerMessageType.Error, ptrnAst.ParametersNames[i].Position);

					// last error should be whole compiled object
					prms.Messages.AddMessage("Failed to compile symbol pattern `{0}`.".Fmt(ptrnAst.Name),
						CompilerMessageType.Error, ptrnAst.Position);
					result = null;
					return false;
				}

				usedNames.Add(name, ptrnAst.ParametersNames[i].Position);
				names[i] = name;
			}

			var namesImm = new ImmutableList<string>(names, true);
			result = new Symbol<string>(ptrnAst.Name, namesImm);
			return true;
		}

		public static bool TryCompile(ImmutableList<Ast.SymbolPattern> ptrnsAst, Dictionary<string, Ast.Position> usedNames, CompilerParameters prms, out SymbolsList<string> result) {

			var compiledSymbols = new Symbol<string>[ptrnsAst.Length];

			for (int i = 0; i < ptrnsAst.Length; i++) {
				if (!TryCompile(ptrnsAst[i], usedNames, prms, out compiledSymbols[i])) {
					result = null;
					return false;
				}
			}

			var compiledSymImm = new ImmutableList<Symbol<string>>(compiledSymbols, true);

			result = new SymbolsList<string>(compiledSymImm);
			return true;
		}


		public static bool TryCompile(Ast.SymbolExprArgs symbolAst, CompilerParameters prms, out Symbol<IExpression> result) {

			ImmutableList<IExpression> compiledExprsImm;

			if (!ExpressionCompiler.TryCompile(symbolAst.Arguments, prms, out compiledExprsImm)) {
				prms.Messages.AddMessage("Failed to compile arguments of symbol `{0}`.".Fmt(symbolAst.Name),
					CompilerMessageType.Error, symbolAst.Position);
				result = null;
				return false;
			}

			result = new Symbol<IExpression>(symbolAst.Name, compiledExprsImm);
			return true;
		}

		public static bool TryCompile(ImmutableList<Ast.SymbolExprArgs> symbolsAst, CompilerParameters prms, out SymbolsList<IExpression> result) {

			var compiledSymbols = new Symbol<IExpression>[symbolsAst.Length];

			for (int i = 0; i < symbolsAst.Length; i++) {
				if (!TryCompile(symbolsAst[i], prms, out compiledSymbols[i])) {
					result = null;
					return false;
				}
			}

			var compiledSymImm = new ImmutableList<Symbol<IExpression>>(compiledSymbols, true);

			result = new SymbolsList<IExpression>(compiledSymImm);
			return true;
		}

	}
}

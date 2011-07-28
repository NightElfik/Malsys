﻿using Malsys.Expressions;

namespace Malsys.Compilers {
	/// <summary>
	/// Thread safe.
	/// </summary>
	public static class VariableDefinitionCompiler {

		public static bool TryCompile(Ast.VariableDefinition varDef, CompilerParametersInternal prms, out VariableDefinition result) {

			IExpression expr;
			if (ExpressionCompiler.TryCompile(varDef.Expression, prms, out expr)) {
				result = new VariableDefinition(varDef.NameId.Name, expr);
				return true;
			}
			else {
				prms.Messages.AddMessage("Failed to compile definition of variable `{0}`.".Fmt(varDef.NameId.Name),
					CompilerMessageType.Error, varDef.Position);
				result = null;
				return false;
			}
		}

		public static bool TryCompile(ImmutableList<Ast.VariableDefinition> varDefs, CompilerParametersInternal prms, out ImmutableList<VariableDefinition> result) {
			var rsltList = new VariableDefinition[varDefs.Length];

			for (int i = 0; i < varDefs.Length; i++) {
				if (!TryCompile(varDefs[i], prms, out rsltList[i])) {
					result = null;
					return false;
				}
			}

			result = new ImmutableList<VariableDefinition>(rsltList, true);
			return true;
		}
	}
}

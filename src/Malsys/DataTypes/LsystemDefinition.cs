using System;
using System.Diagnostics;
using Malsys.Expressions;

namespace Malsys {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class LsystemDefinition {

		public readonly string Name;

		public readonly ImmutableList<OptionalParameter> Parameters;

		public readonly ImmutableList<FunctionDefinition> Functions;
		public readonly ImmutableList<VariableDefinition<IExpression>> Variables;
		public readonly ImmutableList<VariableDefinition<SymbolsList<IExpression>>> Symbols;

		public readonly ImmutableList<RewriteRule> RewriteRules;


		public LsystemDefinition(string name, ImmutableList<OptionalParameter> prms, ImmutableList<RewriteRule> rRules,
				ImmutableList<VariableDefinition<IExpression>> vars, ImmutableList<VariableDefinition<SymbolsList<IExpression>>> syms,
				ImmutableList<FunctionDefinition> funs) {

			Name = name;
			Parameters = prms;
			Functions = funs;
			Variables = vars;
			Symbols = syms;
			RewriteRules = rRules;
		}
	}
}

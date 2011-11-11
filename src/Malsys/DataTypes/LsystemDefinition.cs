using System;
using System.Diagnostics;
using Malsys.Expressions;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IExpression>;
using SymbolVarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SymbolsList<Malsys.Expressions.IExpression>>;

namespace Malsys {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class LsystemDefinition {

		public readonly string Name;

		public readonly ImmutableList<OptionalParameter> Parameters;

		public readonly FunMap Functions;
		public readonly VarMap Variables;
		public readonly SymbolVarMap Symbols;

		public readonly ImmutableList<RewriteRule> RewriteRules;


		public LsystemDefinition(string name, ImmutableList<OptionalParameter> prms, ImmutableList<RewriteRule> rRules,
				VarMap vars, SymbolVarMap syms, FunMap funs) {

			Name = name;
			Parameters = prms;
			Functions = funs;
			Variables = vars;
			Symbols = syms;
			RewriteRules = rRules;
		}
	}
}

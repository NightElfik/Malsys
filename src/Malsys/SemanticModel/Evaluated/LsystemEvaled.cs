using Malsys.SemanticModel.Compiled;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using FunsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.FunctionEvaledParams>;
using SymIntMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>>;
using SymListMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.ImmutableList<Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>>>;

namespace Malsys.SemanticModel.Evaluated {
	public class LsystemEvaled {

		public readonly string Name;

		/// <summary>
		/// Contains global consts from evaluation + parameters + local consts.
		/// </summary>
		public readonly ConstsMap Constants;

		/// <summary>
		/// Contains global functions from evaluation + local functions.
		/// </summary>
		public readonly FunsMap Functions;

		public readonly SymListMap SymbolsConstants;

		public readonly SymIntMap SymbolsInterpretation;

		public readonly ImmutableList<RewriteRule> RewriteRules;

		public readonly ImmutableList<ProcessStatement> ProcessStatements;



		public readonly Ast.LsystemDefinition AstNode;


		public LsystemEvaled(string name, ConstsMap consts, FunsMap funs, SymListMap symbolsConsts, SymIntMap symsInt,
				ImmutableList<RewriteRule> rRules, ImmutableList<ProcessStatement> processStatements, Ast.LsystemDefinition astNode) {

			Name = name;
			Constants = consts;
			Functions = funs;
			SymbolsConstants = symbolsConsts;
			SymbolsInterpretation = symsInt;
			RewriteRules = rRules;
			ProcessStatements = processStatements;
			AstNode = astNode;
		}
	}
}

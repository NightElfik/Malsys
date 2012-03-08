using Malsys.Evaluators;
using Malsys.SemanticModel.Compiled;
using Microsoft.FSharp.Collections;
using SymIntMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.SymbolInterpretationEvaled>;
using SymListMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.ImmutableList<Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>>>;
using ValsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;

namespace Malsys.SemanticModel.Evaluated {
	public class LsystemEvaled {

		public readonly string Name;


		public readonly IExpressionEvaluatorContext ExpressionEvaluatorContext;

		public readonly ValsMap ComponentValuesAssigns;

		public readonly SymListMap ComponentSymbolsAssigns;

		public readonly SymIntMap SymbolsInterpretation;

		public readonly ImmutableList<RewriteRule> RewriteRules;

		public readonly ImmutableList<ProcessStatement> ProcessStatements;



		public readonly Ast.LsystemDefinition AstNode;


		/// <summary>
		/// Creates new empty L-system.
		/// </summary>
		public LsystemEvaled(string name) {

			Name = name;
			ExpressionEvaluatorContext = new ExpressionEvaluatorContext();
			ComponentValuesAssigns = MapModule.Empty<string, IValue>();
			ComponentSymbolsAssigns = MapModule.Empty<string, ImmutableList<Symbol<IValue>>>();
			SymbolsInterpretation = MapModule.Empty<string, SymbolInterpretationEvaled>();
			RewriteRules = ImmutableList<RewriteRule>.Empty;
			ProcessStatements = ImmutableList<ProcessStatement>.Empty;
			AstNode = null;
		}

		public LsystemEvaled(string name, IExpressionEvaluatorContext exprEvalCtxt, ValsMap valuesAssigns, SymListMap symbolsAssigns, SymIntMap symsInt,
				ImmutableList<RewriteRule> rRules, ImmutableList<ProcessStatement> processStatements, Ast.LsystemDefinition astNode = null) {

			Name = name;
			ExpressionEvaluatorContext = exprEvalCtxt;
			ComponentValuesAssigns = valuesAssigns;
			ComponentSymbolsAssigns = symbolsAssigns;
			SymbolsInterpretation = symsInt;
			RewriteRules = rRules;
			ProcessStatements = processStatements;
			AstNode = astNode;
		}
	}
}

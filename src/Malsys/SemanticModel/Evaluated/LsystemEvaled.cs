// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using Malsys.Evaluators;
using Malsys.SemanticModel.Compiled;
using Microsoft.FSharp.Collections;
using SymIntMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.SymbolInterpretationEvaled>;
using SymListMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.ImmutableList<Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>>>;
using ValsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;

namespace Malsys.SemanticModel.Evaluated {
	public class LsystemEvaled {

		public readonly string Name;

		public readonly bool IsAbstract;


		public readonly ImmutableList<LsystemEvaled> BaseLsystems;

		public readonly IExpressionEvaluatorContext ExpressionEvaluatorContext;

		public readonly ValsMap ComponentValuesAssigns;

		public readonly SymListMap ComponentSymbolsAssigns;

		public readonly SymIntMap SymbolsInterpretation;

		public readonly ImmutableList<RewriteRule> RewriteRules;



		public readonly Ast.LsystemDefinition AstNode;


		/// <summary>
		/// Creates new empty L-system.
		/// </summary>
		public LsystemEvaled(string name) {

			Name = name;
			IsAbstract = false;
			BaseLsystems = new ImmutableList<LsystemEvaled>();
			ExpressionEvaluatorContext = new ExpressionEvaluatorContext();
			ComponentValuesAssigns = MapModule.Empty<string, IValue>();
			ComponentSymbolsAssigns = MapModule.Empty<string, ImmutableList<Symbol<IValue>>>();
			SymbolsInterpretation = MapModule.Empty<string, SymbolInterpretationEvaled>();
			RewriteRules = ImmutableList<RewriteRule>.Empty;
			AstNode = null;
		}

		public LsystemEvaled(string name, bool isAbstract, ImmutableList<LsystemEvaled> baseLsystems, IExpressionEvaluatorContext exprEvalCtxt,
				ValsMap valuesAssigns, SymListMap symbolsAssigns, SymIntMap symsInt, ImmutableList<RewriteRule> rRules, Ast.LsystemDefinition astNode = null) {

			Name = name;
			IsAbstract = isAbstract;
			BaseLsystems = baseLsystems;
			ExpressionEvaluatorContext = exprEvalCtxt;
			ComponentValuesAssigns = valuesAssigns;
			ComponentSymbolsAssigns = symbolsAssigns;
			SymbolsInterpretation = symsInt;
			RewriteRules = rRules;
			AstNode = astNode;
		}

	}
}

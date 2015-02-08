using System.Collections.Generic;
using Malsys.Evaluators;
using Malsys.SemanticModel.Compiled;
using Microsoft.FSharp.Collections;
using SymIntMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.SymbolInterpretationEvaled>;
using SymListMap = Microsoft.FSharp.Collections.FSharpMap<string, System.Collections.Generic.List<Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>>>;
using ValsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;

namespace Malsys.SemanticModel.Evaluated {
	public class LsystemEvaled {

		public string Name;
		public bool IsAbstract;

		public List<LsystemEvaled> BaseLsystems;
		public IExpressionEvaluatorContext ExpressionEvaluatorContext;
		public ValsMap ComponentValuesAssigns;
		public SymListMap ComponentSymbolsAssigns;
		public SymIntMap SymbolsInterpretation;
		public List<RewriteRule> RewriteRules;

		public readonly Ast.LsystemDefinition AstNode;


		public LsystemEvaled(Ast.LsystemDefinition astNode) {
			AstNode = astNode;
		}

		/// <summary>
		/// Creates new empty L-system.
		/// </summary>
		public LsystemEvaled(string name) {
			Name = name;
			IsAbstract = false;
			BaseLsystems = new List<LsystemEvaled>();
			ExpressionEvaluatorContext = new ExpressionEvaluatorContext();
			ComponentValuesAssigns = MapModule.Empty<string, IValue>();
			ComponentSymbolsAssigns = MapModule.Empty<string, List<Symbol<IValue>>>();
			SymbolsInterpretation = MapModule.Empty<string, SymbolInterpretationEvaled>();
			RewriteRules = new List<RewriteRule>();
			AstNode = null;
		}


	}
}

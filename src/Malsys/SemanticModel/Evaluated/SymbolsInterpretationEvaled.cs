using Malsys.SemanticModel.Compiled;

namespace Malsys.SemanticModel.Evaluated {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class SymbolInterpretationEvaled {

		public readonly string Symbol;
		public readonly ImmutableList<OptionalParameterEvaled> Parameters;
		public readonly string InstructionName;
		public readonly ImmutableList<IExpression> InstructionParameters;

		public readonly Ast.SymbolsInterpretDef AstNode;


		public SymbolInterpretationEvaled(string symbol, ImmutableList<OptionalParameterEvaled> parameters,
				string instrName, ImmutableList<IExpression> instrParams, Ast.SymbolsInterpretDef astNode = null) {

			Symbol = symbol;
			Parameters = parameters;
			InstructionName = instrName;
			InstructionParameters = instrParams;

			AstNode = astNode;
		}


	}
}

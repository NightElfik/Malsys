using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FunctionDefinition : IInputStatement, ILsystemStatement, IExprInteractiveStatement {

		public readonly Identificator NameId;
		public readonly int ParametersCount;
		public readonly ImmutableListPos<OptionalParameter> Parameters;
		public readonly ImmutableListPos<VariableDefinition> LocalVarDefs;
		public readonly Expression ReturnExpression;
		public readonly ImmutableList<KeywordPos> keywords;


		public FunctionDefinition(Identificator name, ImmutableListPos<OptionalParameter> prms, ImmutableListPos<VariableDefinition> varDefs,
				Expression retExpr, ImmutableList<KeywordPos> kws, Position pos) {

			NameId = name;
			Parameters = prms;
			LocalVarDefs = varDefs;
			ReturnExpression = retExpr;
			keywords = kws;

			ParametersCount = Parameters.Length;
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

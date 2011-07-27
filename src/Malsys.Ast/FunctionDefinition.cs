using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FunctionDefinition : RichExpression, IInputFileStatement, IExprInteractiveStatement {

		public readonly Keyword Keyword;
		public readonly Identificator NameId;
		public readonly int ParametersCount;

		public readonly ImmutableList<OptionalParameter> Parameters;


		public FunctionDefinition(Keyword keyword, Identificator name, IEnumerable<OptionalParameter> prms,
				IEnumerable<VariableDefinition> varDefs, Expression expr, Position pos)
			: base(varDefs, expr, pos) {

			Keyword = keyword;
			NameId = name;
			Parameters = new ImmutableList<OptionalParameter>(prms);

			ParametersCount = Parameters.Length;
		}


		#region IAstVisitable Members

		public new void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

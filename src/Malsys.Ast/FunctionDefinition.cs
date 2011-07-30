using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FunctionDefinition : RichExpression, IInputStatement, ILsystemStatement, IExprInteractiveStatement {

		public readonly Keyword Keyword;
		public readonly Identificator NameId;
		public readonly int ParametersCount;

		public readonly ImmutableListPos<OptionalParameter> Parameters;


		public FunctionDefinition(Keyword keyword, Identificator name, ImmutableListPos<OptionalParameter> prms,
				RichExpression expr, Position pos)
			: base(expr.VariableDefinitions, expr.Expression, pos) {

			Keyword = keyword;
			NameId = name;
			Parameters = prms;

			ParametersCount = Parameters.Length;
		}


		#region IAstVisitable Members

		public new void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

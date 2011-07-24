using System.Collections.Generic;
using System.Linq;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FunctionDefinition : RichExpression, IInputFileStatement, IExprInteractiveStatement {

		public readonly Keyword Keyword;
		public readonly Identificator NameId;
		public readonly int ParametersCount;

		private OptionalParameter[] parameters;


		public FunctionDefinition(Keyword keyword, Identificator name, IEnumerable<OptionalParameter> prms,
				IEnumerable<VariableDefinition> varDefs, Expression expr, Position pos) : base(varDefs, expr, pos) {

			Keyword = keyword;
			NameId = name;
			parameters = prms.ToArray();

			ParametersCount = parameters.Length;
		}

		public OptionalParameter GetOptionalParameter(int i) {
			return parameters[i];
		}


		#region IAstVisitable Members

		public new void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

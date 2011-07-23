using System.Collections.Generic;
using System.Linq;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FunctionDefinition : RichExpression, IInputFileStatement {

		public readonly Keyword Keyword;
		public readonly Identificator Name;
		public readonly int ParametersCount;

		private OptionalParameter[] parameters;


		public FunctionDefinition(Keyword keyword, Identificator name, IEnumerable<OptionalParameter> prms,
				IEnumerable<VariableDefinition> varDefs, Expression expr, Position pos) : base(varDefs, expr, pos) {

			Keyword = keyword;
			Name = name;
			parameters = prms.ToArray();
		}

		public OptionalParameter GetOptionalParameter(int i) {
			return parameters[i];
		}


		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

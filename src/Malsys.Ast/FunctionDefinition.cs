using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FunctionDefinition : IInputStatement, ILsystemStatement, IExprInteractiveStatement {

		public readonly Keyword Keyword;
		public readonly Identificator NameId;
		public readonly int ParametersCount;
		public readonly ImmutableListPos<OptionalParameter> Parameters;
		public readonly RichExpression Body;


		public FunctionDefinition(Keyword keyword, Identificator name, ImmutableListPos<OptionalParameter> prms, RichExpression body, Position pos) {

			Keyword = keyword;
			NameId = name;
			Parameters = prms;
			Body = body;

			ParametersCount = Parameters.Length;
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion
	}
}

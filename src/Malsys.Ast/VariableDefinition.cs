
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class VariableDefinition : IToken, IInputStatement, ILsystemStatement, IExprInteractiveStatement {

		public readonly Keyword Keyword;
		public readonly Identificator NameId;
		public readonly Expression Expression;


		public VariableDefinition(Keyword keyword, Identificator name, Expression expr, Position pos) {
			Keyword = keyword;
			NameId = name;
			Expression = expr;
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion
	}
}

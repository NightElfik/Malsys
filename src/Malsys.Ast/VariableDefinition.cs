
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class VariableDefinition : IToken, IInputStatement, ILsystemStatement, IExprInteractiveStatement {

		public readonly KeywordPos Keyword;
		public readonly Identificator NameId;
		public readonly Expression Expression;


		public VariableDefinition(KeywordPos keyword, Identificator name, Expression expr, Position pos) {
			Keyword = keyword;
			NameId = name;
			Expression = expr;
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

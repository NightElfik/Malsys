
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class VariableDefinition : IToken, IInputFileStatement, ILsystemStatement {

		public readonly Keyword Keyword;
		public readonly Identificator Name;
		public readonly Expression Expression;


		public VariableDefinition(Keyword keyword, Identificator name, Expression expr, Position pos) {
			Keyword = keyword;
			Name = name;
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

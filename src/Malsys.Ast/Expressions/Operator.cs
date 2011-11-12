
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Operator : IExpressionMember {

		public readonly string Syntax;


		public Operator(string syntax, Position pos) {
			Syntax = syntax;
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstExpressionVisitable Members

		public void Accept(IAstExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}


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



		public Position Position { get; private set; }


		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

	}
}

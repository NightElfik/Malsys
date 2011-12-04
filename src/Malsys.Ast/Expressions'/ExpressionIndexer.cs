
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExpressionIndexer : IExpressionMember {

		public readonly Expression Index;


		public ExpressionIndexer(Expression index, Position pos) {
			Index = index;
			Position = pos;
		}



		public Position Position { get; private set; }


		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

	}
}

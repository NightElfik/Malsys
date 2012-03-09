
namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ExpressionIndexer : IExpressionMember {

		public readonly Expression Index;


		public ExpressionIndexer(Expression index, Position pos) {
			Index = index;
			Position = pos;
		}



		public Position Position { get; private set; }


		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.ExpressionIndexer; }
		}

	}
}

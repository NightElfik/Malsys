
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExpressionIndexer : IToken, IExpressionMember {

		public readonly Expression Index;


		public ExpressionIndexer(Expression index, Position pos) {
			Index = index;
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IExpressionMember Members

		public ExpressionMemberType MemberType { get { return ExpressionMemberType.Indexer; } }

		#endregion
	}
}

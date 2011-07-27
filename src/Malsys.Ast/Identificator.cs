
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Identificator : IToken, IAstVisitable, IExpressionMember {

		public readonly string Name;


		public Identificator(string name, Position pos) {
			Name = name;
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

		#region IExpressionMember Members

		public ExpressionMemberType MemberType { get { return ExpressionMemberType.Variable; } }

		#endregion
	}
}

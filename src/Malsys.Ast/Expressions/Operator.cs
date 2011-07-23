
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Operator : IToken, IAstVisitable, IExpressionMember {

		public readonly string Syntax;


		public Operator(string syntax, Position pos) {
			Syntax = syntax;
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

		public ExpressionMemberType MemberType { get { return ExpressionMemberType.Operator; } }

		#endregion
	}
}

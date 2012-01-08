using System;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class EmptyExpression : IExpressionMember {

		public static readonly EmptyExpression Instance = new EmptyExpression(Position.Unknown);


		public EmptyExpression(Position pos) {
			Position = pos;
		}


		public Position Position { get; private set; }


		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

	}
}

using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExpressionFunction : IToken, IAstVisitable, IExpressionMember {

		public readonly Identificator NameId;
		public readonly ImmutableList<Expression> Arguments;


		public ExpressionFunction(Identificator name, IEnumerable<Expression> args, Position pos) {
			NameId = name;
			Arguments = new ImmutableList<Expression>(args);
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

		public ExpressionMemberType MemberType { get { return ExpressionMemberType.Function; } }

		#endregion
	}
}

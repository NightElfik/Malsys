using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExpressionFunction : IToken, IExpressionMember {

		public readonly Identificator NameId;
		public readonly ImmutableListPos<Expression> Arguments;


		public ExpressionFunction(Identificator name, ImmutableListPos<Expression> args, Position pos) {
			NameId = name;
			Arguments = args;
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IExpressionMember Members

		public ExpressionMemberType MemberType { get { return ExpressionMemberType.Function; } }

		#endregion

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

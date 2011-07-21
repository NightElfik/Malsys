using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Malsys.Ast {
	/// <summary>
	/// Expression tree, partially linearized.
	/// </summary>
	public class Expression : IToken, IAstVisitable, IValue {
		public readonly ReadOnlyCollection<IExpressionMember> Members;

		public Expression(IList<IExpressionMember> members, Position pos) {
			Members = new ReadOnlyCollection<IExpressionMember>(members);
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

		#region IValue Members

		public bool IsExpression { get { return true; } }
		public bool IsArray { get { return false; } }

		#endregion
	}
}

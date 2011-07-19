using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Malsys.Ast {
	/// <summary>
	/// Expression tree, partially linearized.
	/// </summary>
	public class Expression : IToken, IAstVisitable, IExpressionMember {
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
	}

	public interface IExpressionMember : IToken, IAstVisitable { }

	public class ExpressionFunction : IToken, IAstVisitable, IExpressionMember {

		public byte Arity {
			get {
				Debug.Assert(Arguments.Count < byte.MaxValue, "Too many arguments.");
				return (byte)Arguments.Count;
			}
		}

		public readonly Identificator NameId;
		public readonly ReadOnlyCollection<Expression> Arguments;

		public ExpressionFunction(Identificator name, IList<Expression> args, Position pos) {
			NameId = name;
			Arguments = new ReadOnlyCollection<Expression>(args);
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
	}
}

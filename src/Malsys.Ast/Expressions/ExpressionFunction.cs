using System.Collections.Generic;
using System.Linq;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExpressionFunction : IToken, IAstVisitable, IExpressionMember {

		public Expression this[int i] { get { return arguments[i]; } }

		public readonly Identificator NameId;
		public readonly int ArgumentsCount;

		private Expression[] arguments;


		public ExpressionFunction(Identificator name, IList<Expression> args, Position pos) {
			NameId = name;
			arguments = args.ToArray();
			Position = pos;

			ArgumentsCount = arguments.Length;
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

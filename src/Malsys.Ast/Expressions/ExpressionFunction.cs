using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Malsys.Ast {
	public class ExpressionFunction : IToken, IAstVisitable, IExpressionMember {

		public IValue this[int i] { get { return arguments[i]; } }

		public readonly int ArgumentsCount;

		public readonly Identificator NameId;
		public readonly IValue[] arguments;

		public ExpressionFunction(Identificator name, IList<IValue> args, Position pos) {
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

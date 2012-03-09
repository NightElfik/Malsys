
namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ExpressionFunction : IExpressionMember {

		public readonly Identificator NameId;
		public readonly ImmutableListPos<Expression> Arguments;


		public ExpressionFunction(Identificator name, ImmutableListPos<Expression> args, Position pos) {
			NameId = name;
			Arguments = args;
			Position = pos;
		}


		public Position Position { get; private set; }


		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.ExpressionFunction; }
		}

	}
}

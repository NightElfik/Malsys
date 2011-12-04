
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExpressionFunction : IExpressionMember {

		public readonly Identificator NameId;
		public readonly ImmutableListPos<Expression> Arguments;


		public ExpressionFunction(Identificator name, ImmutableListPos<Expression> args, Position pos) {
			NameId = name;
			Arguments = args;
			Position = pos;
		}


		public Position Position { get; private set; }


		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

	}
}

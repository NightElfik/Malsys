
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class LsystemSymbol : IToken {

		public readonly string Name;
		public readonly ImmutableListPos<Expression> Arguments;


		public LsystemSymbol(string name, ImmutableListPos<Expression> args, Position pos) {
			Name = name;
			Arguments = args;

			Position = pos;
		}


		public Position Position { get; private set; }

	}
}

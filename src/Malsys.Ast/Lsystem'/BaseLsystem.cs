
namespace Malsys.Ast {
	public class BaseLsystem : IAstNode {

		public readonly Identifier NameId;
		public readonly ListPos<Expression> Arguments;

		public PositionRange Position { get; private set; }


		public BaseLsystem(Identifier name, ListPos<Expression> args, PositionRange pos) {
			NameId = name;
			Arguments = args;

			Position = pos;
		}

	}
}

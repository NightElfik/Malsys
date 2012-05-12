/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class BaseLsystem : IToken {

		public readonly Identificator NameId;
		public readonly ImmutableListPos<Expression> Arguments;


		public BaseLsystem(Identificator name, ImmutableListPos<Expression> args, Position pos) {
			NameId = name;
			Arguments = args;

			Position = pos;
		}


		public Position Position { get; private set; }

	}
}

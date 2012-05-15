/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class BaseLsystem : IAstNode {

		public readonly Identifier NameId;
		public readonly ImmutableListPos<Expression> Arguments;


		public BaseLsystem(Identifier name, ImmutableListPos<Expression> args, PositionRange pos) {
			NameId = name;
			Arguments = args;

			Position = pos;
		}


		public PositionRange Position { get; private set; }

	}
}

/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class LsystemSymbol : IAstNode {

		public readonly string Name;
		public readonly ImmutableListPos<Expression> Arguments;


		public LsystemSymbol(string name, ImmutableListPos<Expression> args, PositionRange pos) {
			Name = name;
			Arguments = args;

			Position = pos;
		}


		public PositionRange Position { get; private set; }

	}
}

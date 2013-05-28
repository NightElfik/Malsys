// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public abstract class NameParamsStatements<TStatement> : IAstNode where TStatement : IAstNode {

		public readonly Identifier NameId;
		public readonly ImmutableListPos<OptionalParameter> Parameters;
		public readonly ImmutableListPos<TStatement> Statements;


		public NameParamsStatements(Identifier name, ImmutableListPos<OptionalParameter> prms,
				ImmutableListPos<TStatement> statements, PositionRange pos) {

			NameId = name;
			Parameters = prms;
			Statements = statements;
			Position = pos;
		}


		public PositionRange Position { get; private set; }

	}
}

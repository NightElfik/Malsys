﻿/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public abstract class NameParamsStatements<TStatement> : IToken where TStatement : IToken {

		public readonly Identificator NameId;
		public readonly ImmutableListPos<OptionalParameter> Parameters;
		public readonly ImmutableListPos<TStatement> Statements;


		public NameParamsStatements(Identificator name, ImmutableListPos<OptionalParameter> prms,
				ImmutableListPos<TStatement> statements, Position pos) {

			NameId = name;
			Parameters = prms;
			Statements = statements;
			Position = pos;
		}


		public Position Position { get; private set; }

	}
}

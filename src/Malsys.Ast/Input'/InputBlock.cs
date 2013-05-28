// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class InputBlock {

		public string SourceName;

		public ImmutableListPos<IInputStatement> Statements;


		public InputBlock(string sourceName, ImmutableListPos<IInputStatement> statements) {

			SourceName = sourceName;
			Statements = statements;

		}

	}
}

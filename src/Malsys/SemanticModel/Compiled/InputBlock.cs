/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.SemanticModel.Compiled {
	public class InputBlock {

		public string SourceName;

		public ImmutableList<IInputStatement> Statements;


		public InputBlock(string sourceName, ImmutableList<IInputStatement> statements) {

			SourceName = sourceName;
			Statements = statements;
		}

	}
}

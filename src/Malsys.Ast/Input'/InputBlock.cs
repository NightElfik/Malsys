using System.Collections.Generic;

namespace Malsys.Ast {
	public class InputBlock {

		public string SourceName;

		public ImmutableListPos<IInputStatement> Statements;


		public InputBlock(string sourceName, ImmutableListPos<IInputStatement> statements) {

			SourceName = sourceName;
			Statements = statements;
		}
	}
}

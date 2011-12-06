using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

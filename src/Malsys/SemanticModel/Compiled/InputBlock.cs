// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;

namespace Malsys.SemanticModel.Compiled {
	public class InputBlock {

		public string SourceName;
		public List<IInputStatement> Statements;

		public readonly Ast.InputBlock AstNode;


		public InputBlock(Ast.InputBlock astNode) {
			AstNode = astNode;
		}

	}
}

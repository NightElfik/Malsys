// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;

namespace Malsys.SemanticModel.Compiled {
	public class BaseLsystem {

		public string Name;
		public List<IExpression> Arguments;

		public readonly Ast.BaseLsystem AstNode;


		public BaseLsystem(Ast.BaseLsystem astNode) {
			AstNode = astNode;
		}

	}
}

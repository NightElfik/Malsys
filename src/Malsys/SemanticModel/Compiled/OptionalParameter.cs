// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using Malsys.SemanticModel.Compiled.Expressions;

namespace Malsys.SemanticModel.Compiled {
	public class OptionalParameter {

		public string Name;
		public IExpression DefaultValue;

		public readonly Ast.OptionalParameter AstNode;


		public OptionalParameter(Ast.OptionalParameter astNode) {
			AstNode = astNode;
		}


		public bool IsOptional { get { return DefaultValue.ExpressionType != ExpressionType.EmptyExpression; } }

	}
}

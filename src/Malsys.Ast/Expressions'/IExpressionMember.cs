// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {

	/// <remarks>
	/// All expression members should be immutable.
	/// </remarks>
	public interface IExpressionMember : IAstNode {

		ExpressionMemberType MemberType { get; }

	}


	public enum ExpressionMemberType{

		EmptyExpression,
		ExpressionBracketed,
		ExpressionFunction,
		ExpressionIndexer,
		ExpressionsArray,
		FloatConstant,
		Identifier,
		Operator,

	}

}

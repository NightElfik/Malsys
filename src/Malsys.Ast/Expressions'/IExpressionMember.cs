
namespace Malsys.Ast {

	/// <remarks>
	/// All expression members should be immutable.
	/// </remarks>
	public interface IExpressionMember : IToken {

		ExpressionMemberType MemberType { get; }

	}


	public enum ExpressionMemberType{

		EmptyExpression,
		ExpressionBracketed,
		ExpressionFunction,
		ExpressionIndexer,
		ExpressionsArray,
		FloatConstant,
		Identificator,
		Operator,

	}

}

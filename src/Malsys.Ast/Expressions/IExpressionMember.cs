
namespace Malsys.Ast {

	/// <summary>
	/// All expression members should be immutable.
	/// </summary>
	public interface IExpressionMember : IToken {
		ExpressionMemberType MemberType { get; }
	}

	public enum ExpressionMemberType {
		Constant, Variable, Array, Operator, Indexer, Function, BracketedExpression
	}
}

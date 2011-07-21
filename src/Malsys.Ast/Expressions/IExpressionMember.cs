
namespace Malsys.Ast {
	public interface IExpressionMember : IToken, IAstVisitable {
		bool IsConstant { get; }
		bool IsVariable { get; }
		bool IsArray { get; }
		bool IsOperator { get; }
		bool IsFunction { get; }
		bool IsIndexer { get; }
		bool IsBracketedExpression { get; }
	}
}

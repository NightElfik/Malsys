
namespace Malsys.Ast {
	public interface IValue : IToken, IExpressionInteractiveStatement {
		bool IsExpression { get; }
		bool IsArray { get; }
	}
}

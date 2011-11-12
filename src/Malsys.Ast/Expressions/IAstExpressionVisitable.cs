
namespace Malsys.Ast {
	public interface IAstExpressionVisitable {
		void Accept(IAstExpressionVisitor visitor);
	}
}

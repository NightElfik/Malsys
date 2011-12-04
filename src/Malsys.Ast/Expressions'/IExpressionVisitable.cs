
namespace Malsys.Ast {
	public interface IExpressionVisitable {
		void Accept(IExpressionVisitor visitor);
	}
}

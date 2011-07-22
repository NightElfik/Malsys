
namespace Malsys.Expressions {
	public interface IExpressionVisitable {
		void Accept(IExpressionVisitor visitor);
	}
}

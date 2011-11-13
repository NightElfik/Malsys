
namespace Malsys.SemanticModel.Compiled.Expressions {
	public interface IExpressionVisitable {
		void Accept(IExpressionVisitor visitor);
	}
}

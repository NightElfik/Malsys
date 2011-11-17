using Malsys.SemanticModel.Compiled.Expressions;

namespace Malsys.SemanticModel.Compiled {

	public interface IExpression : IExpressionVisitable {

		bool IsEmptyExpression { get; }

	}
}

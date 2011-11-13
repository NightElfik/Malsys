using Malsys.SemanticModel.Compiled.Expressions;

namespace Malsys.SemanticModel.Compiled {

	public interface IExpression : IExpressionVisitable, IBindable {

		public bool IsEmpty { get; }

	}
}

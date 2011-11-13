
namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Indexer : IExpression, IExpressionVisitable {

		public readonly IExpression Array;
		public readonly IExpression Index;

		public Indexer(IExpression array, IExpression index) {
			Array = array;
			Index = index;
		}


		#region IExpression Members

		public bool IsEmpty { get { return false; } }

		#endregion

		#region IExpressionVisitable Members

		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}


namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExprVariable : IExpression, IExpressionVisitable {

		public readonly string Name;


		public ExprVariable(string name) {
			Name = name;
		}


		public override string ToString() {
			return Name;
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


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

		public bool IsEmptyExpression { get { return false; } }

		#endregion

		#region IExpressionVisitable Members

		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}


	public static class ExprVariableExtensions {
		public static ExprVariable ToVar(this string name) {
			return new ExprVariable(name);
		}
	}
}

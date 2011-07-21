
namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Variable : IPostfixExpressionMember {
		public readonly string Name;

		public Variable(string name) {
			Name = name;
		}


		public override string ToString() {
			return Name;
		}

		#region IPostfixExpressionMember Members

		public bool IsConstant { get { return false; } }
		public bool IsArray { get { return false; } }
		public bool IsVariable { get { return true; } }
		public bool IsEvaluable { get { return false; } }
		public bool IsUnknownFunction { get { return false; } }

		#endregion
	}
}

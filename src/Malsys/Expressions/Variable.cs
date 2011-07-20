
namespace Malsys.Expressions {
	public class Variable : IPostfixExpressionMember {
		public readonly string Name;

		public Variable(string name) {
			Name = name;
		}

		#region IPostfixExpressionMember Members

		public bool IsConstant { get { return false; } }
		public bool IsVariable { get { return true; } }
		public bool IsEvaluable { get { return false; } }

		#endregion
	}
}

using Malsys.Expressions;

namespace Malsys {
	public class VariableDefinition {
		public readonly string Name;
		public readonly IExpression Value;

		public VariableDefinition(string name, IExpression val) {
			Name = name;
			Value = val;
		}
	}
}

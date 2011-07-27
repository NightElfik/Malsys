using Malsys.Expressions;

namespace Malsys {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Variable {

		public readonly string Name;
		public readonly IValue Value;

		public Variable(string name, IValue val) {
			Name = name;
			Value = val;
		}
	}
}

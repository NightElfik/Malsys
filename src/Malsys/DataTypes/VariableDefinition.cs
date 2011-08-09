using Malsys.Expressions;

namespace Malsys {
	/// <summary>
	/// Immutable.
	/// </summary>
	/// <typeparam name="TValue">Should be immutable.</typeparam>
	public class VariableDefinition<TValue> {

		public readonly string Name;
		public readonly TValue Value;

		public VariableDefinition(string name, TValue val) {
			Name = name;
			Value = val;
		}
	}
}

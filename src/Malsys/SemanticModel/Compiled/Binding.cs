
namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Binding : ILsystemStatement {

		public readonly string Name;

		public readonly IBindable Value;

		public readonly BindingType BindingType;


		public Binding(string name, IBindable value, BindingType bindType) {

			Name = name;
			Value = value;
			BindingType = bindType;
		}
	}

	public enum BindingType {
		Expression,
		Function,
		SymbolList,
	}
}

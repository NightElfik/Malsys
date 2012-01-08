
namespace Malsys.SemanticModel.Evaluated {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class OptionalParameterEvaled {

		public readonly string Name;
		public readonly IValue DefaultValue;


		public OptionalParameterEvaled(string name) {
			Name = name;
			DefaultValue = null;
		}

		public OptionalParameterEvaled(string name, IValue defaultValue) {
			Name = name;
			DefaultValue = defaultValue;
		}


		public bool IsOptional { get { return DefaultValue != null; } }
	}
}

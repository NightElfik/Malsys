
namespace Malsys {
	public class OptionalParameter {
		public string Name { get; set; }
		public IVariableValue DefaultValue { get; set; }

		public bool IsOptional {
			get { return DefaultValue != null; }
		}


		public OptionalParameter() {
			Name = null;
			DefaultValue = null;
		}

		public OptionalParameter(string name) {
			Name = name;
			DefaultValue = null;
		}

		public OptionalParameter(string name, IVariableValue defaultValue) {
			Name = name;
			DefaultValue = defaultValue;
		}
	}
}

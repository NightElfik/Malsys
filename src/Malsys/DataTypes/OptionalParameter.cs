using Malsys.Expressions;

namespace Malsys {
	public class OptionalParameter {
		public string Name { get; set; }
		public IExpression DefaultValue { get; set; }

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

		public OptionalParameter(string name, IExpression defaultValue) {
			Name = name;
			DefaultValue = defaultValue;
		}
	}
}

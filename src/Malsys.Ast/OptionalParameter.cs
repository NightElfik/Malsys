
namespace Malsys.Ast {
	public class OptionalParameter {
		public readonly Identificator Name;
		public readonly IValue OptionalValue;

		public OptionalParameter(Identificator name) {
			Name = name;
			OptionalValue = null;
		}

		public OptionalParameter(Identificator name, IValue optionalValue) {
			Name = name;
			OptionalValue = optionalValue;
		}
	}
}

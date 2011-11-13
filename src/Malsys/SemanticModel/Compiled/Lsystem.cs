
namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Lsystem {

		public readonly string Name;

		public readonly ImmutableList<OptionalParameter> Parameters;

		public readonly ImmutableList<ILsystemStatement> Statements;


		public Lsystem(string name, ImmutableList<OptionalParameter> prms, ImmutableList<ILsystemStatement> statements) {

			Name = name;
			Parameters = prms;
			Statements = statements;
		}
	}
}

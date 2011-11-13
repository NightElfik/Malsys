
namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Lsystem : IBindable {

		public readonly ImmutableList<OptionalParameter> Parameters;

		public readonly ImmutableList<ILsystemStatement> Statements;

		public readonly Ast.Binding AstNode;


		public Lsystem(ImmutableList<OptionalParameter> prms, ImmutableList<ILsystemStatement> statements, Ast.Binding astNode) {

			Parameters = prms;
			Statements = statements;
			AstNode = astNode;
		}
	}
}

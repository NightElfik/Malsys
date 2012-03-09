using System.Collections.Generic;

namespace Malsys.SemanticModel {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class Symbol<T> {


		public readonly string Name;
		public readonly ImmutableList<T> Arguments;

		public readonly Ast.LsystemSymbol AstSymbol;


		public Symbol(string name) {
			Name = name;
			Arguments = ImmutableList<T>.Empty;
		}

		public Symbol(string name, IEnumerable<T> args) {
			Name = name;
			Arguments = new ImmutableList<T>(args);
		}

		public Symbol(string name, ImmutableList<T> args) {
			Name = name;
			Arguments = args;
		}

		public Symbol(string name, ImmutableList<T> args, Ast.LsystemSymbol astSymbol) {
			Name = name;
			Arguments = args;
			AstSymbol = astSymbol;
		}
	}
}

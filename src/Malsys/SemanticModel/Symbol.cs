/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Collections.Generic;

namespace Malsys.SemanticModel {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class Symbol<T> {


		public readonly string Name;
		public readonly ImmutableList<T> Arguments;

		public readonly Ast.LsystemSymbol AstNode;


		public Symbol(string name) {
			Name = name;
			Arguments = ImmutableList<T>.Empty;
		}

		public Symbol(string name, IEnumerable<T> args) {
			Name = name;
			Arguments = new ImmutableList<T>(args);
		}

		public Symbol(string name, ImmutableList<T> args, Ast.LsystemSymbol astNode = null) {
			Name = name;
			Arguments = args;
			AstNode = astNode;
		}
	}
}

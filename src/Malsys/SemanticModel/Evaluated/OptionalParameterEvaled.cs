/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.SemanticModel.Evaluated {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class OptionalParameterEvaled {

		public readonly string Name;
		public readonly IValue DefaultValue;

		public readonly Ast.OptionalParameter AstNode;


		public OptionalParameterEvaled(string name, Ast.OptionalParameter astNode = null) {

			Name = name;
			DefaultValue = null;

			AstNode = astNode;

		}

		public OptionalParameterEvaled(string name, IValue defaultValue, Ast.OptionalParameter astNode = null) {

			Name = name;
			DefaultValue = defaultValue;

			AstNode = astNode;
		}


		public bool IsOptional { get { return DefaultValue != null; } }


	}
}

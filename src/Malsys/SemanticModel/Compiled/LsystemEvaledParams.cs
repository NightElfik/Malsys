/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using Malsys.SemanticModel.Evaluated;

namespace Malsys.SemanticModel.Compiled {
	public class LsystemEvaledParams {

		public readonly string Name;
		public readonly bool IsAbstract;
		public readonly ImmutableList<OptionalParameterEvaled> Parameters;
		public readonly ImmutableList<BaseLsystem> BaseLsystems;
		public readonly ImmutableList<ILsystemStatement> Statements;
		public readonly Ast.LsystemDefinition AstNode;


		public LsystemEvaledParams(string name, bool isAbstract, ImmutableList<OptionalParameterEvaled> prms,
				ImmutableList<BaseLsystem> baseLsystems, ImmutableList<ILsystemStatement> statements, Ast.LsystemDefinition astNode) {

			Name = name;
			IsAbstract = isAbstract;
			Parameters = prms;
			BaseLsystems = baseLsystems;
			Statements = statements;
			AstNode = astNode;
		}

	}
}

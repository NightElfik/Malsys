// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using Malsys.SemanticModel.Compiled;

namespace Malsys.SemanticModel.Evaluated {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessStatementEvaled {

		public readonly string TargetLsystemName;

		public readonly ImmutableList<IValue> Arguments;

		public readonly string ProcessConfigName;

		public readonly ImmutableList<ProcessComponentAssignment> ComponentAssignments;

		public readonly ImmutableList<ILsystemStatement> AdditionalLsystemStatements;


		public readonly Ast.ProcessStatement AstNode;


		public ProcessStatementEvaled(string targetLsystemName, ImmutableList<IValue> arguments, string processConfigName,
				ImmutableList<ProcessComponentAssignment> componentAssignments, ImmutableList<ILsystemStatement> additionalLsysStats, Ast.ProcessStatement astNode = null) {

			TargetLsystemName = targetLsystemName;
			Arguments = arguments;
			ProcessConfigName = processConfigName;
			ComponentAssignments = componentAssignments;
			AdditionalLsystemStatements = additionalLsysStats;

			AstNode = astNode;
		}


	}
}

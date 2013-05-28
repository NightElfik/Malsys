// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.SemanticModel.Compiled {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessComponentsConnection {

		public readonly bool IsVirtual;
		public readonly string SourceName;
		public readonly string TargetName;
		public readonly string TargetInputName;

		public Ast.ProcessConfigConnection AstNode;


		public ProcessComponentsConnection(bool isVirtual, string sourceName, string targetName, string targetInputName, Ast.ProcessConfigConnection astNode = null) {

			IsVirtual = isVirtual;
			SourceName = sourceName;
			TargetName = targetName;
			TargetInputName = targetInputName;

			AstNode = astNode;

		}

	}
}

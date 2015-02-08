using System.Collections.Generic;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.SemanticModel.Compiled {
	public class LsystemEvaledParams {

		public string Name;
		public bool IsAbstract;
		public List<OptionalParameterEvaled> Parameters;
		public List<BaseLsystem> BaseLsystems;
		public List<ILsystemStatement> Statements;

		public readonly Ast.LsystemDefinition AstNode;


		public LsystemEvaledParams(Ast.LsystemDefinition astNode) {
			AstNode = astNode;
		}

	}
}


namespace Malsys.SemanticModel.Compiled {
	public class ProcessContainer {

		public string Name;
		public string TypeName;
		public string DefaultTypeName;

		public readonly Ast.ProcessContainer AstNode;


		public ProcessContainer(Ast.ProcessContainer astNode) {
			AstNode = astNode;
		}

	}
}


namespace Malsys.Ast {
	public class InputBlock {

		public string SourceName;

		public ListPos<IInputStatement> Statements;


		public InputBlock(string sourceName, ListPos<IInputStatement> statements) {
			SourceName = sourceName;
			Statements = statements;
		}

	}
}


namespace Malsys.Ast {
	public class Identificator : Token {
		public readonly string Name;

		public Identificator(string name, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			Name = name;
		}
	}
}

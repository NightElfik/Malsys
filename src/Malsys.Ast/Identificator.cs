
namespace Malsys.Ast {
	public class Identificator : Token, IAstVisitable {
		public readonly string Name;

		public Identificator(string name, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			Name = name;
		}

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

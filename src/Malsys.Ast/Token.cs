
namespace Malsys.Ast {
	public abstract class Token {
		public readonly int BeginLine;
		public readonly int BeginColumn;
		public readonly int EndLine;
		public readonly int EndColumn;

		public Token(int beginLine, int beginColumn, int endLine, int endColumn) {
			BeginLine = beginLine;
			BeginColumn = beginColumn;
			EndLine = endLine;
			EndColumn = endColumn;
		}
	}
}

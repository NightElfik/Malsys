
namespace Malsys.Ast {
	public class VariableDefinition : Token, IInputFileStatement, ILsystemStatement {
		public readonly Keyword Keyword;
		public readonly Identificator Name;
		public readonly Expression Expression;

		public VariableDefinition(Keyword keyword, Identificator name, Expression expression, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			Keyword = keyword;
			Name = name;
			Expression = expression;
		}
	}
}

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Malsys.Ast {
	public interface ILsystemStatement : IAstVisitable { }

	public class Lsystem : Token, IInputFileStatement {
		public readonly Keyword Keyword;
		public readonly Identificator Name;
		public readonly ReadOnlyCollection<ILsystemStatement> Statements;

		public Lsystem(Keyword keyword, Identificator name, IList<ILsystemStatement> statements, int endLine, int endColumn)
			: base(keyword.BeginLine, keyword.BeginColumn, endLine, endColumn) {

			Keyword = keyword;
			Name = name;
			Statements = new ReadOnlyCollection<ILsystemStatement>(statements);
		}

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

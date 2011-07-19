using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Malsys.Ast {
	public interface ILsystemStatement : IAstVisitable { }

	public class Lsystem : IToken, IInputFileStatement {
		public readonly Keyword Keyword;
		public readonly Identificator Name;
		public readonly ReadOnlyCollection<ILsystemStatement> Statements;

		public Lsystem(Keyword keyword, Identificator name, IList<ILsystemStatement> statements, Position pos) {
			Keyword = keyword;
			Name = name;
			Statements = new ReadOnlyCollection<ILsystemStatement>(statements);
			Position = pos;
		}

		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

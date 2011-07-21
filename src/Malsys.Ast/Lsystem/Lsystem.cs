using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Malsys.Ast {
	public interface ILsystemStatement : IAstVisitable { }

	public class Lsystem : IToken, IInputFileStatement {
		public readonly Keyword Keyword;
		public readonly Identificator Name;
		/// <summary>
		/// If it is null, in input were not parenthesis (no params at all).
		/// If it is array with zero length, empy parenthesis were in input.
		/// </summary>
		public readonly ReadOnlyCollection<OptionalParameter> Parameters;
		public readonly ReadOnlyCollection<ILsystemStatement> Statements;

		public Lsystem(Keyword keyword, Identificator name, IList<ILsystemStatement> statements, Position pos) {
			Keyword = keyword;
			Name = name;
			Parameters = null;
			Statements = new ReadOnlyCollection<ILsystemStatement>(statements);
			Position = pos;
		}

		public Lsystem(Keyword keyword, Identificator name, IList<OptionalParameter> parameters, IList<ILsystemStatement> statements, Position pos) {
			Keyword = keyword;
			Name = name;
			Parameters = new ReadOnlyCollection<OptionalParameter>(parameters);
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

using System.Collections.Generic;

namespace Malsys.Ast {
	public interface ILsystemStatement : IAstVisitable { }

	/// <summary>
	/// Immutable.
	/// </summary>
	public class Lsystem : IToken, IInputFileStatement {

		public readonly Keyword Keyword;
		public readonly Identificator NameId;

		public readonly ImmutableList<OptionalParameter> Parameters;
		public readonly ImmutableList<ILsystemStatement> Statements;


		public Lsystem(Keyword keyword, Identificator name, IEnumerable<ILsystemStatement> satetmnts, Position pos) {
			Keyword = keyword;
			NameId = name;
			Parameters = ImmutableList<OptionalParameter>.Empty;
			Statements = new ImmutableList<ILsystemStatement>(satetmnts);
			Position = pos;
		}

		public Lsystem(Keyword keyword, Identificator name, IEnumerable<OptionalParameter> prms, IEnumerable<ILsystemStatement> satetmnts, Position pos) {
			Keyword = keyword;
			NameId = name;
			Parameters = new ImmutableList<OptionalParameter>(prms);
			Statements = new ImmutableList<ILsystemStatement>(satetmnts);
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

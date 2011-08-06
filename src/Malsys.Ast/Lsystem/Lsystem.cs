using System.Collections.Generic;

namespace Malsys.Ast {
	public interface ILsystemStatement : IToken { }

	/// <summary>
	/// Immutable.
	/// </summary>
	public class Lsystem : IToken, IInputStatement {

		public readonly Keyword Keyword;
		public readonly Identificator NameId;

		public readonly ImmutableListPos<OptionalParameter> Parameters;
		public readonly ImmutableList<ILsystemStatement> Statements;


		public Lsystem(Keyword keyword, Identificator name, ImmutableListPos<OptionalParameter> prms, IEnumerable<ILsystemStatement> satetmnts, Position pos) {
			Keyword = keyword;
			NameId = name;
			Parameters = prms;
			Statements = new ImmutableList<ILsystemStatement>(satetmnts);
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion
	}
}

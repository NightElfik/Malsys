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
		public readonly ImmutableListPos<ILsystemStatement> Body;


		public Lsystem(Keyword keyword, Identificator name, ImmutableListPos<OptionalParameter> prms, ImmutableListPos<ILsystemStatement> body, Position pos) {
			Keyword = keyword;
			NameId = name;
			Parameters = prms;
			Body = body;
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

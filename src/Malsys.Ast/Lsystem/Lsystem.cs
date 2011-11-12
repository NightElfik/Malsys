using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Lsystem : IToken, IInputStatement {

		public readonly Identificator NameId;

		public readonly ImmutableListPos<OptionalParameter> Parameters;
		public readonly ImmutableListPos<ILsystemStatement> Body;


		public Lsystem(Identificator name, ImmutableListPos<OptionalParameter> prms, ImmutableListPos<ILsystemStatement> body, Position pos) {

			NameId = name;
			Parameters = prms;
			Body = body;
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstInputVisitable Members

		public void Accept(IAstInputVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Lsystem : IToken, IInputStatement {

		public readonly Identificator NameId;

		public readonly ImmutableListPos<OptionalParameter> Parameters;
		public readonly ImmutableListPos<ILsystemStatement> Statements;


		public Lsystem(Identificator name, ImmutableListPos<OptionalParameter> prms, ImmutableListPos<ILsystemStatement> statements, Position pos) {

			NameId = name;
			Parameters = prms;
			Statements = statements;
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

using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Lsystem : IBindable, IInputStatement {

		public readonly ImmutableListPos<OptionalParameter> Parameters;
		public readonly ImmutableListPos<ILsystemStatement> Statements;


		public Lsystem(ImmutableListPos<OptionalParameter> prms, ImmutableListPos<ILsystemStatement> statements, Position pos) {

			Parameters = prms;
			Statements = statements;
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IBindableVisitable Members

		public void Accept(IBindableVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion

		#region IAstInputVisitable Members

		public void Accept(IAstInputVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

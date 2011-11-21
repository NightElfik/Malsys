using System;
using System.Diagnostics.Contracts;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Identificator : IToken, IExpressionMember {

		public readonly string Name;


		public Identificator(string name, Position pos) {

			Contract.Requires<ArgumentNullException>(name != null);

			Name = name;
			Position = pos;
		}

		[ContractInvariantMethod]
		private void objectInvariant() {
			Contract.Invariant(Name != null);
		}


		public override string ToString() {
			return Name;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstExpressionVisitable Members

		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

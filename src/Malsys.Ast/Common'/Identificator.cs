using System;
using System.Diagnostics.Contracts;

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class Identificator : IToken, IExpressionMember {

		public static readonly Identificator Empty = new Identificator("", Position.Unknown);

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


		public bool IsEmpty { get { return Name.Length == 0; } }


		public override string ToString() {
			return Name;
		}


		public Position Position { get; private set; }


		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.Identificator; }
		}

	}
}

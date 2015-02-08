using System;
using System.Diagnostics.Contracts;

namespace Malsys.Ast {
	public class Identifier : IAstNode, IExpressionMember {

		public string Name;

		public PositionRange Position { get; private set; }
		public bool IsEmpty { get { return Name.Length == 0; } }


		public Identifier(string name, PositionRange pos) {

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


		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.Identifier; }
		}

	}
}

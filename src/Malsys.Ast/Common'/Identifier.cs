// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Diagnostics.Contracts;

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class Identifier : IAstNode, IExpressionMember {

		public static readonly Identifier Empty = new Identifier("", PositionRange.Unknown);

		public readonly string Name;


		public Identifier(string name, PositionRange pos) {

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


		public PositionRange Position { get; private set; }


		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.Identifier; }
		}

	}
}

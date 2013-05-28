// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class OptionalParameter : IAstNode {

		public readonly Identifier NameId;
		public readonly Expression DefaultValue;


		public OptionalParameter(Identifier name, Expression defValue, PositionRange pos) {
			NameId = name;
			DefaultValue = defValue;
			Position = pos;
		}


		public bool IsOptional { get { return !DefaultValue.IsEmpty; } }


		public PositionRange Position { get; private set; }

	}
}

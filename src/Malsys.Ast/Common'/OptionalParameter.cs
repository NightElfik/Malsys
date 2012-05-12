/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class OptionalParameter : IToken {

		public readonly Identificator NameId;
		public readonly Expression DefaultValue;


		public OptionalParameter(Identificator name, Expression defValue, Position pos) {
			NameId = name;
			DefaultValue = defValue;
			Position = pos;
		}


		public bool IsOptional { get { return !DefaultValue.IsEmpty; } }


		public Position Position { get; private set; }

	}
}

﻿
namespace Malsys.Ast {
	public class OptionalParameter : IAstNode {

		public Identifier NameId;
		public Expression DefaultValue;

		public PositionRange Position { get; private set; }
		public bool IsOptional { get { return !DefaultValue.IsEmpty; } }


		public OptionalParameter(Identifier name, Expression defValue, PositionRange pos) {
			NameId = name;
			DefaultValue = defValue;
			Position = pos;
		}

	}
}

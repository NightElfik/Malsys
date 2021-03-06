﻿
namespace Malsys.Ast {
	public class LsystemSymbol : IAstNode {

		public string Name;
		public ListPos<Expression> Arguments;

		public PositionRange Position { get; private set; }


		public LsystemSymbol(string name, ListPos<Expression> args, PositionRange pos) {
			Name = name;
			Arguments = args;

			Position = pos;
		}

	}
}

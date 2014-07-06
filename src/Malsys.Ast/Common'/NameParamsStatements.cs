// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	public abstract class NameParamsStatements<TStatement> : IAstNode where TStatement : IAstNode {

		public Identifier NameId;
		public ListPos<OptionalParameter> Parameters;
		public ListPos<TStatement> Statements;

		public PositionRange Position { get; private set; }


		public NameParamsStatements(Identifier name, ListPos<OptionalParameter> prms,
				ListPos<TStatement> statements, PositionRange pos) {

			NameId = name;
			Parameters = prms;
			Statements = statements;
			Position = pos;
		}

	}
}

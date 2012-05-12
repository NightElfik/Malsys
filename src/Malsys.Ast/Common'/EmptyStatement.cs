/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class EmptyStatement : IStatement, IInputStatement, ILsystemStatement, IProcessConfigStatement {


		public EmptyStatement(Position pos) {
			Position = pos;
		}


		public Position Position { get; private set; }



		InputStatementType IInputStatement.StatementType {
			get { return InputStatementType.EmptyStatement; }
		}

		LsystemStatementType ILsystemStatement.StatementType {
			get { return LsystemStatementType.EmptyStatement; }
		}

		ProcessConfigStatementType IProcessConfigStatement.StatementType {
			get { return ProcessConfigStatementType.EmptyStatement; }
		}

	}
}

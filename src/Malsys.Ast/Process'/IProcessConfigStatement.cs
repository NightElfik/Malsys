/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {

	public interface IProcessConfigStatement : IStatement {

		ProcessConfigStatementType StatementType { get; }

	}


	public enum ProcessConfigStatementType {

		EmptyStatement,
		ProcessComponent,
		ProcessContainer,
		ProcessConfigConnection,

	}

}

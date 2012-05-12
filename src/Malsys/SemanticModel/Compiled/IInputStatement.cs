/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.SemanticModel.Compiled {

	public interface IInputStatement {

		InputStatementType StatementType { get; }

	}

	public enum InputStatementType {
		Constant,
		Function,
		Lsystem,
		ProcessStatement,
		ProcessConfiguration,
	}

}

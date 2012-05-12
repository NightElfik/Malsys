/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {

	public interface IFunctionStatement : IStatement {

		FunctionStatementType StatementType { get; }

	}


	public enum FunctionStatementType {

		ConstantDefinition,
		Expression,

	}

}

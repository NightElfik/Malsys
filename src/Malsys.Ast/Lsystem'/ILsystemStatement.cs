/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {

	public interface ILsystemStatement : IStatement {

		LsystemStatementType StatementType { get; }

	}


	public enum LsystemStatementType {

		EmptyStatement,
		ConstantDefinition,
		SymbolsConstDefinition,
		SymbolsInterpretDef,
		FunctionDefinition,
		RewriteRule,

	}

}

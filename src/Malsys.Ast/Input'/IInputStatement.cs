// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {

	public interface IInputStatement : IStatement {

		InputStatementType StatementType { get; }

	}


	public enum InputStatementType {

		EmptyStatement,
		ConstantDefinition,
		FunctionDefinition,
		LsystemDefinition,
		ProcessStatement,
		ProcessConfigurationDefinition,

	}

}

// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.SemanticModel.Compiled {

	public interface ILsystemStatement {

		LsystemStatementType StatementType { get; }

	}

	public enum LsystemStatementType {

		Constant,
		Function,
		SymbolsConstant,
		RewriteRule,
		SymbolsInterpretation,

	}

}

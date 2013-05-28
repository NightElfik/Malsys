﻿// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.SemanticModel.Compiled {

	public interface IFunctionStatement {

		FunctionStatementType StatementType { get; }

	}


	public enum FunctionStatementType {

		ConstantDefinition,
		ReturnExpression,

	}

}

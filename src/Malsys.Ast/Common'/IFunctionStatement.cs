﻿
namespace Malsys.Ast {

	public interface IFunctionStatement : IStatement {

		FunctionStatementType StatementType { get; }

	}


	public enum FunctionStatementType {

		ConstantDefinition,
		Expression,

	}

}

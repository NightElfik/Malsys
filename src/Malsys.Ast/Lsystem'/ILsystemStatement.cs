
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

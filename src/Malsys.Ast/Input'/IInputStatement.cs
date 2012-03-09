
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

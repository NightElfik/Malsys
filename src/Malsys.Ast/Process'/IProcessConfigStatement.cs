
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

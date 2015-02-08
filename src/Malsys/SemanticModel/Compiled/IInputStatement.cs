
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


namespace Malsys.SemanticModel.Compiled {
	public interface ILsystemStatement {

		LsystemStatementType StatementType { get; }

	}

	public enum LsystemStatementType {
		Binding,
		RewriteRule,
	}
}

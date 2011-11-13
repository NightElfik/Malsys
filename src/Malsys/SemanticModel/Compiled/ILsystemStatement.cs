
namespace Malsys.SemanticModel.Compiled {
	public interface ILsystemStatement {

		public LsystemStatementType StatementType { get; }

	}

	public enum LsystemStatementType {
		Binding,
		RewriteRule,
	}
}

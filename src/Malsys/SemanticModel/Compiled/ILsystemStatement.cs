
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

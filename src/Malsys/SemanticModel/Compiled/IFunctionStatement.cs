
namespace Malsys.SemanticModel.Compiled {

	public interface IFunctionStatement {

		FunctionStatementType StatementType { get; }

	}

	public enum FunctionStatementType {
		ConstantDefinition,
		ReturnExpression,
	}

}

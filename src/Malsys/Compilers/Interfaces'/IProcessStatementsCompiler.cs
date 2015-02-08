using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	public interface IProcessStatementsCompiler : ICompiler<Ast.ProcessStatement, ProcessStatement>,
			ICompiler<Ast.ProcessConfigurationDefinition, ProcessConfigurationStatement> {
	}
}

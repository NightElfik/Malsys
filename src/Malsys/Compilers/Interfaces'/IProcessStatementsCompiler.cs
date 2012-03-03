using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	interface IProcessStatementsCompiler : ICompiler<Ast.ProcessStatement, ProcessStatement>,
			ICompiler<Ast.ProcessConfigurationDefinition, ProcessConfigurationStatement> {
	}
}

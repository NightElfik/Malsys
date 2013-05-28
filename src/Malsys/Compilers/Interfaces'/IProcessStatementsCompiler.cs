// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	public interface IProcessStatementsCompiler : ICompiler<Ast.ProcessStatement, ProcessStatement>,
			ICompiler<Ast.ProcessConfigurationDefinition, ProcessConfigurationStatement> {
	}
}

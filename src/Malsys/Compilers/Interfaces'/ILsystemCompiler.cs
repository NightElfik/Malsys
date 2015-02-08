using System.Collections.Generic;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	public interface ILsystemCompiler : ICompiler<Ast.LsystemDefinition, Lsystem>, ICompiler<Ast.ILsystemStatement, ILsystemStatement> {

		/// <remarks>
		/// Explicitly defined method for compiling	statements list which will skip empty statements.
		/// </remarks>
		List<ILsystemStatement> CompileList(IEnumerable<Ast.ILsystemStatement> statementsList, IMessageLogger logger);

	}
}

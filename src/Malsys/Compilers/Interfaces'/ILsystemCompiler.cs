// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	public interface ILsystemCompiler : ICompiler<Ast.LsystemDefinition, Lsystem>, ICompiler<Ast.ILsystemStatement, ILsystemStatement> {

		/// <remarks>
		/// Explicitly defined method for compiling	statements list which will skip empty statements.
		/// </remarks>
		ImmutableList<ILsystemStatement> CompileList(IEnumerable<Ast.ILsystemStatement> statementsList, IMessageLogger logger);

	}
}

// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	public interface IParametersCompiler : ICompiler<Ast.OptionalParameter, OptionalParameter> {

		/// <summary>
		/// Compiles list of optional parameters.
		/// Checks if no mandatory parameters are after optional. Also checks is parameters names are unique.
		/// </summary>
		List<OptionalParameter> CompileList(IEnumerable<Ast.OptionalParameter> list, IMessageLogger logger);

	}
}

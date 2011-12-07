using System.Collections.Generic;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	public interface IParametersCompiler : ICompiler<Ast.OptionalParameter, OptionalParameter> {

		/// <summary>
		/// Compiles list of optional parameters.
		/// Checks if no mandatory parameters are after optional. Also checks is parameters names are unique.
		/// </summary>
		ImmutableList<OptionalParameter> CompileList(IList<Ast.OptionalParameter> list, IMessageLogger logger);

	}
}

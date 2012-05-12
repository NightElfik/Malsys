/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Collections.Generic;

namespace Malsys.Compilers {
	public interface ICompilerConstantsProvider {

		/// <summary>
		/// Tries to get constant with name equal to given string.
		/// </summary>
		bool TryGetConstant(string name, out CompilerConstant result);

		IEnumerable<CompilerConstant> GetAllConstants();

	}
}

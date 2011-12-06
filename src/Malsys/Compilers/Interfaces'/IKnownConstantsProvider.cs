using System.Collections.Generic;
using Malsys.Compilers.Expressions;

namespace Malsys.Compilers {
	public interface IKnownConstantsProvider {

		/// <summary>
		/// Tries to get constant with name equal to given string.
		/// </summary>
		bool TryGet(string name, out KnownConstant result);

		IEnumerable<KnownConstant> GetAllConstants();

	}
}

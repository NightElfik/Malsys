using System.Collections.Generic;

namespace Malsys.Compilers {
	public interface IKnownConstantsProvider {

		/// <summary>
		/// Tries to get constant with name equal to given string.
		/// </summary>
		bool TryGet(string name, out double result);

		IEnumerable<double> GetAllConstants();

	}
}

using System.Collections.Generic;
using Malsys.Resources;

namespace Malsys.Compilers {
	public interface IOperatorsProvider {

		bool TryGet(string syntax, bool unary, out OperatorCore result);

		IEnumerable<OperatorCore> GetAllOperators();

	}
}

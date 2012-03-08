using System.Collections.Generic;
using Malsys.Resources;

namespace Malsys.Compilers {
	public interface IKnownOperatorsProvider {

		bool TryGet(string syntax, bool unary, out OperatorCore result);

		IEnumerable<OperatorCore> GetAllOperators();

	}
}

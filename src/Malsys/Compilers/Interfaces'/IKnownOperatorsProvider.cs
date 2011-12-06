using System.Collections.Generic;
using Malsys.Compilers.Expressions;

namespace Malsys.Compilers {
	public interface IKnownOperatorsProvider {

		bool TryGet(string syntax, byte arity, out OperatorCore result);

		IEnumerable<OperatorCore> GetAllOperators();

	}
}

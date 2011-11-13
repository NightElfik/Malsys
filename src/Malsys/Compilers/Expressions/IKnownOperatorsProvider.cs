using System.Collections.Generic;

namespace Malsys.Compilers.Expressions {
	public interface IKnownOperatorsProvider {

		bool TryGet(string syntax, byte arity, out OperatorCore result);

		IEnumerable<OperatorCore> GetAllOperators();

	}
}

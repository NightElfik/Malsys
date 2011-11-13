using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Malsys.Compilers.Expressions {

	[ContractClass(typeof(IKnownFunctionsProviderContract))]
	public interface IKnownFunctionsProvider {

		bool TryGet(string syntax, int paramsCount, out FunctionCore result);

		IEnumerable<FunctionCore> GetAllFunctions();

	}

	[ContractClassFor(typeof(IKnownFunctionsProvider))]
	abstract class IKnownFunctionsProviderContract : IKnownFunctionsProvider {

		#region IKnownFunctionsProvider Members

		public bool TryGet(string syntax, int paramsCount, out FunctionCore result) {
			Contract.Requires<ArgumentNullException>(syntax != null);
			Contract.Requires<ArgumentOutOfRangeException>(paramsCount >= 0 && paramsCount != FunctionCore.AnyParamsCount);
			Contract.Ensures(paramsCount == Contract.ValueAtReturn(out result).ParametersCount);
			throw new Exception();
		}

		#endregion
	}
}

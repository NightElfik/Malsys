using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Malsys.Compilers.Expressions;

namespace Malsys.Compilers {

	[ContractClass(typeof(IKnownFunctionsProviderContract))]
	public interface IKnownFunctionsProvider {

		bool TryGet(string syntax, int paramsCount, out FunctionCore result);

		IEnumerable<FunctionCore> GetAllFunctions();

	}


	[ContractClassFor(typeof(IKnownFunctionsProvider))]
	abstract class IKnownFunctionsProviderContract : IKnownFunctionsProvider {

		public bool TryGet(string syntax, int paramsCount, out FunctionCore result) {
			Contract.Requires<ArgumentNullException>(syntax != null);
			Contract.Requires<ArgumentOutOfRangeException>(paramsCount >= 0 && paramsCount != FunctionCore.AnyParamsCount);
			Contract.Ensures(Contract.Result<bool>() ? paramsCount == Contract.ValueAtReturn(out result).ParametersCount : true);
			throw new NotImplementedException();
		}

		public IEnumerable<FunctionCore> GetAllFunctions() {
			Contract.Ensures(Contract.Result<IEnumerable<FunctionCore>>() != null);
			throw new NotImplementedException();
		}

	}
}

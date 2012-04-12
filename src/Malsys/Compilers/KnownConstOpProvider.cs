using System.Collections.Generic;
using Malsys.Resources;
using StringBool = System.Tuple<string, bool>;

namespace Malsys.Compilers {
	public class KnownConstOpProvider : ICompilerConstantsProvider, ICompilerConstantsContainer, IOperatorsProvider, IOperatorsContainer {


		private Dictionary<string, CompilerConstant> constCache = new Dictionary<string, CompilerConstant>();
		private Dictionary<StringBool, OperatorCore> opCache = new Dictionary<StringBool, OperatorCore>();



		public bool TryGetConstant(string name, out CompilerConstant result) {
			return constCache.TryGetValue(name.ToLowerInvariant(), out result);
		}

		public IEnumerable<CompilerConstant> GetAllConstants() {
			return constCache.Values;
		}

		public void AddCompilerConstant(CompilerConstant constant) {
			constCache[constant.Name] = constant;
		}



		public bool TryGet(string syntax, bool unary, out OperatorCore result) {
			return opCache.TryGetValue(new StringBool(syntax, unary), out result);
		}

		public IEnumerable<OperatorCore> GetAllOperators() {
			return opCache.Values;
		}

		public void AddOperator(OperatorCore op) {
			opCache[new StringBool(op.Syntax, op.IsUnary)] = op;
		}


	}
}

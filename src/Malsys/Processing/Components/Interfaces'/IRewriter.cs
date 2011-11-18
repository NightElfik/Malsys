using Malsys.Expressions;
using Malsys.SemanticModel.Evaluated;
using Malsys.Evaluators;

namespace Malsys.Processing.Components {
	public interface IRewriter : ISymbolProcessor {

		ISymbolProcessor OutputProcessor { set; }

		[UserSettable]
		IValue RandomSeed { set; }
	}
}

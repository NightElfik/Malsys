using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components {
	public interface IRewriterIterator : ISymbolProcessor, IProcessStarter {

		IRewriter Rewriter { set; }

		ISymbolProcessor OutputProcessor { set; }

		[UserSettable]
		ImmutableList<Symbol<IValue>> Axiom { set; }

		[UserSettable]
		IValue Iterations { set; }

	}
}

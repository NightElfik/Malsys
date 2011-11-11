using Malsys.Expressions;

namespace Malsys.Processing.Components {
	public interface IRewriter : ISymbolProcessor {

		ISymbolProcessor OutputProcessor { set; }

		ProcessContext Context { set; }

		[UserSettable]
		IValue RandomSeed { set; }

	}
}

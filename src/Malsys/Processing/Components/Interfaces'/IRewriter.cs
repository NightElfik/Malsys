using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components {
	public interface IRewriter : ISymbolProcessor {

		ISymbolProcessor OutputProcessor { set; }

		[UserSettable]
		IValue RandomSeed { set; }
	}
}

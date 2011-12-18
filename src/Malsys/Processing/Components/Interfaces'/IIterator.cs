
using Malsys.SemanticModel.Evaluated;
namespace Malsys.Processing.Components {
	public interface IIterator : ISymbolProvider, IProcessStarter {

		ISymbolProvider SymbolProvider { set; }

		ISymbolProvider AxiomProvider { set; }

		ISymbolProcessor OutputProcessor { set; }

		IValue Iterations { set; }

	}
}

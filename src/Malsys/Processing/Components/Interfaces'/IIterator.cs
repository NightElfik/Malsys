
using Malsys.SemanticModel.Evaluated;
using Malsys.SemanticModel;
namespace Malsys.Processing.Components {
	[Component("Generic iterator", ComponentGroupNames.Iterators)]
	public interface IIterator : ISymbolProvider, IProcessStarter {

		[UserConnectable]
		ISymbolProvider SymbolProvider { set; }

		[UserConnectable]
		ISymbolProvider AxiomProvider { set; }

		[UserConnectable]
		ISymbolProcessor OutputProcessor { set; }

		[UserSettable]
		Constant Iterations { set; }
	}
}

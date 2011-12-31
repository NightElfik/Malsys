
using Malsys.SemanticModel.Evaluated;
namespace Malsys.Processing.Components {
	[Component("Generic iterator", ComponentGroupNames.Iterators)]
	public interface IIterator : ISymbolProvider, IProcessStarter {

		[UserConnectable]
		ISymbolProvider SymbolProvider { set; }

		[UserConnectable]
		ISymbolProvider AxiomProvider { set; }

		[UserConnectable]
		ISymbolProcessor OutputProcessor { set; }

		[UserSettable(IsMandatory = true)]
		IValue Iterations { set; }
	}
}

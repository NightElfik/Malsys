using System.Collections.Generic;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components {
	public interface ISymbolProvider : IEnumerable<Symbol<IValue>>, IProcessComponent {

	}
}

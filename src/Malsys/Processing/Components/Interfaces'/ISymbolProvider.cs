using System.Collections.Generic;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components {
	[Component("Generic Symbol provider", ComponentGroupNames.Common)]
	public interface ISymbolProvider : IEnumerable<Symbol<IValue>>, IProcessComponent {

	}
}

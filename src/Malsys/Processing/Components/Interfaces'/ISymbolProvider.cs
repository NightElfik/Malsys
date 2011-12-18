using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components {
	public interface ISymbolProvider : IEnumerable<Symbol<IValue>>, IComponent {

	}
}

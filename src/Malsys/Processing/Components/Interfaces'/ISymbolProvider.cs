// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components {
	/// <summary>
	///	Symbol providers are components that are called to receive symbols.
	///	Processing is passive, their work is initiated by asking for L-system symbols.
	/// </summary>
	/// <name>Symbol provider component interface</name>
	/// <group>Common</group>
	public interface ISymbolProvider : IEnumerable<Symbol<IValue>>, IProcessComponent {

	}
}

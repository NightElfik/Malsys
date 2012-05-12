/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components {
	/// <summary>
	///	Symbol processors are components that are called to process symbols.
	///	Processing is passive, their work is initiated by sending L-system symbols to them.
	/// </summary>
	/// <name>Symbol processor component interface</name>
	/// <group>Common</group>
	public interface ISymbolProcessor : IProcessComponent {

		void ProcessSymbol(Symbol<IValue> symbol);

	}
}

/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Common {
	/// <summary>
	///	Provides symbol property called Axiom which serve as initial string of symbols of L-system.
	/// </summary>
	/// <name>Axiom provider</name>
	/// <group>Common</group>
	public class AxiomProvider : SymbolProvider {


		/// <summary>
		/// Storage for axiom.
		/// Value is provided to connected component.
		/// </summary>
		[AccessName("axiom")]
		[UserSettableSybols]
		public ImmutableList<Symbol<IValue>> Axiom { set { Symbols = value; } }


	}
}

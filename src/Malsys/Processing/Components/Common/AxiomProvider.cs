/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Common {
	/// <summary>
	///	Provides a symbol property called Axiom which serves as an initial string of symbols of an L-system.
	/// </summary>
	/// <name>Axiom provider</name>
	/// <group>Common</group>
	public class AxiomProvider : SymbolProvider {


		/// <summary>
		/// Initial string of symbols.
		/// The value is provided to the connected component.
		/// </summary>
		[AccessName("axiom")]
		[UserSettableSybols]
		public ImmutableList<Symbol<IValue>> Axiom { set { Symbols = value; } }


		public override void Reset() {
			base.Reset();
			Axiom = ImmutableList<Symbol<IValue>>.Empty;
		}


	}
}

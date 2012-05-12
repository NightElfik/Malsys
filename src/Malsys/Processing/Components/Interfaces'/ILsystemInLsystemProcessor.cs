/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Common {
	/// <summary>
	///	This is special interface component for interpreting L-system symbol as another L-system.
	/// </summary>
	/// <name>Inner L-system processor interface</name>
	/// <group>Special</group>
	public interface ILsystemInLsystemProcessor : IComponent {

		void ProcessLsystem(string name, string configName, IValue[] args);

	}
}

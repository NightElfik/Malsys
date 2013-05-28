// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Processing.Components {
	/// <summary>
	/// Interpreters are responsible for interpreting symbols of L-system.
	/// </summary>
	/// <name>Interpreter interface</name>
	/// <group>Interpreters</group>
	public interface IInterpreter : IProcessComponent {

		IRenderer Renderer { set; }

	}
}

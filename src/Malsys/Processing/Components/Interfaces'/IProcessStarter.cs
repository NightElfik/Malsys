using System;

namespace Malsys.Processing.Components {
	/// <summary>
	///	Process starters are specialized components that starts processing.
	///	This type of component can by only one per process configuration.
	/// </summary>
	/// <name>Starter component interface</name>
	/// <group>Common</group>
	public interface IProcessStarter : IComponent {

		void Start(bool doMeasure);

		void Abort();

	}
}

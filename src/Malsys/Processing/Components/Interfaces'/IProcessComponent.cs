using System;

namespace Malsys.Processing.Components {
	/// <summary>
	///	Process components should be connected to other components and they
	///	receives signals of begin and end of processing.
	/// </summary>
	/// <name>Process component interface</name>
	/// <group>Common</group>
	public interface IProcessComponent : IComponent {

		/// <summary>
		/// Retrieved after component initialization.
		/// </summary>
		bool RequiresMeasure { get; }


		void BeginProcessing(bool measuring);

		void EndProcessing();

	}

}

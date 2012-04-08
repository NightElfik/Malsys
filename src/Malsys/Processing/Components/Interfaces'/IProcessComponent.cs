using System;

namespace Malsys.Processing.Components {
	public interface IProcessComponent : IComponent {

		/// <summary>
		/// Retrieved after component initialization.
		/// </summary>
		bool RequiresMeasure { get; }


		void BeginProcessing(bool measuring);

		void EndProcessing();

	}

}

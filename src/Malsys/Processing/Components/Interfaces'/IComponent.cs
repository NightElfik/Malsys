using System;

namespace Malsys.Processing.Components {
	[Component("Generic component", ComponentGroupNames.Common)]
	public interface IComponent {

		/// <summary>
		/// Retrieved after component initialization.
		/// </summary>
		bool RequiresMeasure { get; }


		void Initialize(ProcessContext ctxt);

		void Cleanup();


		void BeginProcessing(bool measuring);

		void EndProcessing();

	}

}

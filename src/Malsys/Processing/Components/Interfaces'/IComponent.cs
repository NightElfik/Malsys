using System;

namespace Malsys.Processing.Components {
	[Component("Generic component", ComponentGroupNames.Common)]
	public interface IComponent {

		void Initialize(ProcessContext ctxt);

		void Cleanup();

	}

}

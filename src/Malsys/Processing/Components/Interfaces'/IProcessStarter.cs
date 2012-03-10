using System;

namespace Malsys.Processing.Components {
	public interface IProcessStarter : IComponent {

		void Start(bool doMeasure);

		void Abort();

	}
}

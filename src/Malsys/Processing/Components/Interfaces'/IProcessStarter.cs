using System;

namespace Malsys.Processing.Components {
	public interface IProcessStarter {

		void Start(bool doMeasure, TimeSpan timeout);

		void Abort();

	}
}

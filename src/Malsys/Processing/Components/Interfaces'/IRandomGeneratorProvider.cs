using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.Processing.Components {
	public interface IRandomGeneratorProvider : IProcessComponent {

		IRandomGenerator GetRandomGenerator();

	}
}

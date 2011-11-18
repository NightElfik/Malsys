using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.Processing.Components {
	public interface IComponent {

		ProcessContext Context { set; }


		void BeginProcessing(bool measuring);

		void EndProcessing();

	}
}

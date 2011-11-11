using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Malsys.Processing.Components {
	public interface IRenderer {

		ProcessContext Context { set; }

		void BeginRendering();

		void EndRendering();

	}
}

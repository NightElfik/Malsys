using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Malsys.Renderers {
	public interface IRenderer {

		void Initialize();

		void EndRendering();

		string GetResultsFilePaths();

	}
}

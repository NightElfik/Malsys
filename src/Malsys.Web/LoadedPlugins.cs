using System.Collections.Generic;
using System.Reflection;

namespace Malsys.Web {
	public class LoadedPlugins : ImmutableList<Assembly> {

		public LoadedPlugins(IList<Assembly> plugins)
			: base(plugins) { }


	}
}

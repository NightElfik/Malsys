using Malsys.Evaluators;
using Malsys.SemanticModel.Compiled;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing {
	public class BaseLsystemResolver : IBaseLsystemResolver {

		FSharpMap<string, LsystemEvaledParams> lsystems;


		public BaseLsystemResolver() {
			lsystems = MapModule.Empty<string, LsystemEvaledParams>();
		}

		public BaseLsystemResolver(FSharpMap<string, LsystemEvaledParams> lsystems) {
			this.lsystems = lsystems;
		}


		public LsystemEvaledParams Resolve(string name, IMessageLogger logger) {
			return lsystems.TryGetValue(name, null);
		}

	}
}

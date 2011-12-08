using Malsys.SemanticModel.Compiled;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using FunsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.FunctionEvaledParams>;
using LsysMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.LsystemEvaledParams>;
using ProcConfsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.ProcessConfiguration>;

namespace Malsys.SemanticModel.Evaluated {
	public class InputBlock {

		public readonly ConstsMap GlobalConstants;
		public readonly FunsMap GlobalFunctions;
		public readonly LsysMap Lsystems;
		public readonly ProcConfsMap ProcessConfigurations;
		public readonly ImmutableList<ProcessStatement> ProcessStatements;


		public InputBlock(ConstsMap consts, FunsMap funs, LsysMap lsystems, ProcConfsMap procConfigs, ImmutableList<ProcessStatement> procStats) {

			GlobalConstants = consts;
			GlobalFunctions = funs;
			Lsystems = lsystems;
			ProcessConfigurations = procConfigs;
			ProcessStatements = procStats;
		}
	}
}

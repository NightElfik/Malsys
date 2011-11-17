using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FunsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.FunctionEvaledParams>;
using LsysMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.LsystemEvaledParams>;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;

namespace Malsys.SemanticModel.Evaluated {
	public class InputBlock {

		public readonly ConstsMap GlobalConstants;
		public readonly FunsMap GlobalFunctions;
		public readonly LsysMap Lsystems;


		public InputBlock(ConstsMap consts, FunsMap funs, LsysMap lsystems) {

			GlobalConstants = consts;
			GlobalFunctions = funs;
			Lsystems = lsystems;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.FunctionEvaledParams>;
using LsysMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.LsystemEvaledParams>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;

namespace Malsys.SemanticModel.Evaluated {
	public class InputBlock {

		public readonly VarMap GlobalVariables;

		public readonly FunMap GlobalFunctions;

		public readonly LsysMap Lsystems;


		public InputBlock(VarMap vars, FunMap funs, LsysMap lsystems) {

			GlobalVariables = vars;
			GlobalFunctions = funs;
			Lsystems = lsystems;
		}
	}
}

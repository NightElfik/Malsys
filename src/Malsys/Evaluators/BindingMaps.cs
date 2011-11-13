using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.FunctionEvaledParams>;
using LsysMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.LsystemEvaledParams>;
using SymMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.SymbolsListVal>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;

namespace Malsys.Evaluators {
	public class BindingMaps {

		public FunMap Functions;
		public LsysMap Lsystems;
		public SymMap Symbols;
		public VarMap Variables;


		public BindingMaps() {

		}

		public BindingMaps(BindingType bindType) {

			if ((bindType | BindingType.Function) != 0) {
				Functions = MapModule.Empty<string, FunctionEvaledParams>();
			}

			if ((bindType | BindingType.Lsystem) != 0) {
				Lsystems = MapModule.Empty<string, LsystemEvaledParams>();
			}

			if ((bindType | BindingType.SymbolList) != 0) {
				Symbols = MapModule.Empty<string, SymbolsListVal>();
			}

			if ((bindType | BindingType.Expression) != 0) {
				Variables = MapModule.Empty<string, IValue>();
			}

		}

	}
}

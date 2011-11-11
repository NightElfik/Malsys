using Malsys.Expressions;
using Microsoft.FSharp.Collections;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using LsystemMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.LsystemDefinition>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Processing {
	public class InputData {

		public LsystemMap Lsystems { get; private set; }

		public VarMap GlobalVariables { get; private set; }
		public FunMap GlobalFunctions { get; private set; }


		public InputData() {

			Lsystems = MapModule.Empty<string, LsystemDefinition>();
			GlobalFunctions = MapModule.Empty<string, FunctionDefinition>();
			GlobalVariables = MapModule.Empty<string, IValue>();
		}

		public void Add(InputBlock inputBlock) {

			foreach (var lsys in inputBlock.Lsystems) {
				Lsystems = Lsystems.Add(lsys.Name, lsys);
			}

			foreach (var funDef in inputBlock.Functions) {
				GlobalFunctions = GlobalFunctions.Add(funDef.Name, funDef);
			}

			foreach (var varDef in inputBlock.Variables) {
				GlobalVariables = GlobalVariables.Add(varDef.Name, varDef.Value);
			}
		}

		public void Add(LsystemDefinition lsystem) {
			Lsystems = Lsystems.Add(lsystem.Name, lsystem);
		}
	}
}

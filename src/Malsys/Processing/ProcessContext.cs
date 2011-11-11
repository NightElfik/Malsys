using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Expressions;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Processing {
	public class ProcessContext {

		public FilesManager FilesManager { get; private set; }

		public LsystemDefinition MainLsystem { get; private set; }

		public InputData InputData { get; private set; }

		/// <summary>
		/// All variables in current scope (global + lsystem's).
		/// </summary>
		public VarMap Variables { get; private set; }

		/// <summary>
		/// All functions in current scope (global + lsystem's).
		/// </summary>
		public FunMap Functions { get; private set; }


		public ProcessContext(string mainLsystemName, FilesManager filesManager, InputData data) {
			FilesManager = filesManager;
			InputData = data;

			MainLsystem = InputData.Lsystems[mainLsystemName];

			Variables = data.GlobalVariables.AddRange(Variables);
			Functions = data.GlobalFunctions.AddRange(MainLsystem.Functions);

		}
	}
}

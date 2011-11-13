using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Expressions;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.FunctionEvaledParams>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using Malsys.SemanticModel.Evaluated;
using Malsys.Evaluators;

namespace Malsys.Processing {
	public class ProcessContext {

		public LsystemEvaled Lsystem { get; private set; }

		public FilesManager FilesManager { get; private set; }

		public InputBlock InputData { get; private set; }

		public ExpressionEvaluator ExpressionEvaluator { get; private set; }


		public ProcessContext(LsystemEvaled lsystem, FilesManager filesManager, InputBlock data, ExpressionEvaluator exprEal) {

			Lsystem = lsystem;
			FilesManager = filesManager;
			InputData = data;
			ExpressionEvaluator = exprEal;
		}
	}
}

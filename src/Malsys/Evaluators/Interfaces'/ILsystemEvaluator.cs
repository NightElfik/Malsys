using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using FunsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.FunctionEvaledParams>;
using Malsys.SemanticModel.Evaluated;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Evaluators {
	public interface ILsystemEvaluator {

		LsystemEvaled Evaluate(LsystemEvaledParams lsystem, IList<IValue> arguments, ConstsMap consts, FunsMap funs);

	}
}

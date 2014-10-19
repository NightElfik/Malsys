// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;
using Malsys.Evaluators;
using LsysMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.LsystemEvaledParams>;
using ProcConfsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.ProcessConfigurationStatement>;

namespace Malsys.SemanticModel.Evaluated {
	public class InputBlockEvaled {

		public IExpressionEvaluatorContext ExpressionEvaluatorContext;
		public LsysMap Lsystems;
		public ProcConfsMap ProcessConfigurations;
		public List<ProcessStatementEvaled> ProcessStatements = new List<ProcessStatementEvaled>();

		public string SourceName;



		public void Append(InputBlockEvaled inputBlock) {
			ExpressionEvaluatorContext = ExpressionEvaluatorContext.MergeWith(inputBlock.ExpressionEvaluatorContext);
			Lsystems = Lsystems.AddRange(inputBlock.Lsystems);
			ProcessConfigurations = ProcessConfigurations.AddRange(inputBlock.ProcessConfigurations);
			ProcessStatements.AddRange(inputBlock.ProcessStatements);
		}

		public InputBlockEvaled ShallowClone() {
			return new InputBlockEvaled() {
				ExpressionEvaluatorContext = ExpressionEvaluatorContext,
				Lsystems = Lsystems,
				ProcessConfigurations = ProcessConfigurations,
				ProcessStatements = new List<ProcessStatementEvaled>(ProcessStatements),
			};
		}


	}
}

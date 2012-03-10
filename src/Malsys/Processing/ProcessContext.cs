using System;
using Malsys.Evaluators;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing {
	public class ProcessContext {

		public readonly LsystemEvaled Lsystem;

		public readonly IOutputProvider OutputProvider;

		public readonly InputBlockEvaled InputData;

		public readonly IEvaluatorsContainer EvaluatorsContainer;

		public readonly IExpressionEvaluatorContext ExpressionEvaluatorContext;

		public readonly IComponentResolver ComponentResolver;

		public readonly FSharpMap<string, ConfigurationComponent> ComponentGraph;

		public readonly TimeSpan ProcessingTimeLimit;

		public readonly IMessageLogger Logger;


		public ProcessContext(LsystemEvaled lsystem, IOutputProvider outputProvider, InputBlockEvaled data, IEvaluatorsContainer evaluatorsContainer,
				IExpressionEvaluatorContext exprEvalCtxt, IComponentResolver componentResolver, TimeSpan processingTimeLimit,
				FSharpMap<string, ConfigurationComponent> componentGraph, IMessageLogger logger) {

			Lsystem = lsystem;
			OutputProvider = outputProvider;
			InputData = data;
			EvaluatorsContainer = evaluatorsContainer;
			ExpressionEvaluatorContext = exprEvalCtxt;
			ComponentResolver = componentResolver;
			ProcessingTimeLimit = processingTimeLimit;
			ComponentGraph = componentGraph;
			Logger = logger;

		}


	}
}

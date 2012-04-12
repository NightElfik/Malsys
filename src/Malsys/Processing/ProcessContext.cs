using System;
using Malsys.Evaluators;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;
using Malsys.Processing.Components;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Malsys.Processing {
	public class ProcessContext {

		public readonly LsystemEvaled Lsystem;

		public readonly IOutputProvider OutputProvider;

		public readonly InputBlockEvaled InputData;

		public readonly IEvaluatorsContainer EvaluatorsContainer;

		public readonly IExpressionEvaluatorContext ExpressionEvaluatorContext;

		public readonly IComponentMetadataResolver ComponentResolver;

		public readonly FSharpMap<string, ConfigurationComponent> ComponentGraph;

		public readonly TimeSpan ProcessingTimeLimit;

		public readonly IMessageLogger Logger;


		public ProcessContext(LsystemEvaled lsystem, IOutputProvider outputProvider, InputBlockEvaled data, IEvaluatorsContainer evaluatorsContainer,
				IExpressionEvaluatorContext exprEvalCtxt, IComponentMetadataResolver componentResolver, TimeSpan processingTimeLimit,
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


		public KeyValuePair<string, ConfigurationComponent>? FindComponent(IComponent instance) {

			Contract.Requires<ArgumentNullException>(instance != null);
			Contract.Ensures(Contract.Result<KeyValuePair<string, ConfigurationComponent>?>() != null
				? Contract.Result<KeyValuePair<string, ConfigurationComponent>?>().Value.Value.Component == instance
				: true);

			foreach (var compKvp in ComponentGraph) {
				if (compKvp.Value.Component == instance) {
					return compKvp;
				}
			}

			return null;

		}


	}
}

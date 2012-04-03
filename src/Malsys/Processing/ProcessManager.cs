using System;
using System.Collections.Generic;
using System.Linq;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.Processing.Components.Common;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace Malsys.Processing {
	public class ProcessManager {

		private readonly ICompilersContainer compiler;
		private readonly IEvaluatorsContainer evaluator;
		private readonly IComponentResolver componentResolver;

		private readonly ProcessConfigurationBuilder configBuilder = new ProcessConfigurationBuilder();


		public ProcessManager(ICompilersContainer compilersContainer, IEvaluatorsContainer evaluatorsContainer, IComponentResolver componentResolver) {

			compiler = compilersContainer;
			evaluator = evaluatorsContainer;
			this.componentResolver = componentResolver;
		}


		public InputBlockEvaled CompileAndEvaluateInput(string src, string sourcName, IMessageLogger logger) {

			using (var errBlock = logger.StartErrorLoggingBlock()) {

				var inCompiled = compiler.CompileInput(src, sourcName, logger);

				if (errBlock.ErrorOccurred) {
					return null;
				}

				return evaluator.TryEvaluateInput(inCompiled, evaluator.ExpressionEvaluatorContext, logger);
			}

		}

		public void DumpConstants(InputBlockEvaled inBlockEvaled, IOutputProvider outputProvider, IMessageLogger logger) {

			new ConstantsDumper().DumpConstants(inBlockEvaled, outputProvider, logger, inBlockEvaled.SourceName);

		}

		public void ProcessInput(InputBlockEvaled inBlockEvaled, IOutputProvider outputProvider, IMessageLogger logger, TimeSpan timeout) {

			foreach (var processStat in inBlockEvaled.ProcessStatements) {

				var lsystemsToProcess = getLsystemsToProcess(processStat, inBlockEvaled, logger);
				if (lsystemsToProcess == null) {
					continue;
				}

				// create components graph -- graph is same for all processed L-systems with this configuration
				var compGraph = buildComponentsGraph(processStat, inBlockEvaled, logger);
				if (compGraph == null) {
					continue;
				}

				// add variables and functions from components that can be called before init to ExpressionEvaluatorContext
				var eec = inBlockEvaled.ExpressionEvaluatorContext;
				eec = configBuilder.AddComponentsGettableVariables(compGraph, eec, true);
				eec = configBuilder.AddComponentsCallableFunctions(compGraph, eec, true);


				foreach (var lsystem in lsystemsToProcess) {

					ProcessConfiguration procConfig;
					using (var errBlock = logger.StartErrorLoggingBlock()) {

						var lsysEvaled = evaluator.TryEvaluateLsystem(lsystem, processStat.Arguments, eec, logger);
						if (errBlock.ErrorOccurred) {
							continue;
						}

						// set settable properties on components
						configBuilder.SetAndCheckUserSettableProperties(compGraph, lsysEvaled.ComponentValuesAssigns, lsysEvaled.ComponentSymbolsAssigns, logger);

						// add gettable variables which can not be get before initialization
						var newEec = lsysEvaled.ExpressionEvaluatorContext;
						newEec = configBuilder.AddComponentsGettableVariables(compGraph, newEec, false);
						newEec = configBuilder.AddComponentsCallableFunctions(compGraph, newEec, false);

						var context = new ProcessContext(lsysEvaled, outputProvider, inBlockEvaled, evaluator,
							newEec, componentResolver, timeout, compGraph, logger);

						// initialize components with ExpressionEvaluatorContext -- components can call themselves
						configBuilder.InitializeComponents(compGraph, context, logger);

						procConfig = configBuilder.CreateConfiguration(compGraph, logger);
						if (errBlock.ErrorOccurred) {
							continue;
						}
					}

					try {
						procConfig.StarterComponent.Start(procConfig.RequiresMeasure);
					}
					catch (EvalException ex) {
						logger.LogMessage(IEvaluatorsContainerExtensions.Message.EvalFailed, ex.GetFullMessage());
					}
					catch (InterpretationException ex) {
						logger.LogMessage(IEvaluatorsContainerExtensions.Message.EvalFailed, ex.Message);
					}

					configBuilder.CleanupComponents(compGraph, logger);

				}

			}

		}


		private IEnumerable<LsystemEvaledParams> getLsystemsToProcess(ProcessStatementEvaled processStat, InputBlockEvaled inBlockEvaled, IMessageLogger logger) {

			if (processStat.TargetLsystemName.Length == 0) {
				return inBlockEvaled.Lsystems.Select(x => x.Value);
			}
			else {
				LsystemEvaledParams lsys;
				if (!inBlockEvaled.Lsystems.TryGetValue(processStat.TargetLsystemName, out lsys)) {
					logger.LogMessage(Message.LsysNotDefined, processStat.TargetLsystemName);
					return null;
				}
				return new LsystemEvaledParams[] { lsys };
			}

		}

		private FSharpMap<string, ConfigurationComponent> buildComponentsGraph(ProcessStatementEvaled processStat,
				InputBlockEvaled inBlockEvaled, IMessageLogger logger) {

			var maybeConfig = inBlockEvaled.ProcessConfigurations.TryFind(processStat.ProcessConfiName);
			if (OptionModule.IsNone(maybeConfig)) {
				logger.LogMessage(Message.UndefinedProcessConfig, processStat.AstNode.ProcessConfiNameId.Position, processStat.ProcessConfiName);
				return null;
			}

			return configBuilder.BuildConfigurationComponentsGraph(maybeConfig.Value, processStat.ComponentAssignments, componentResolver, logger);

		}


		public enum Message {

			[Message(MessageType.Error, "Undefined process configuration name `{0}`.")]
			UndefinedProcessConfig,
			[Message(MessageType.Error, "Failed to evaluate process statement, L-system `{0}` is not defined.")]
			LsysNotDefined,
			[Message(MessageType.Error, "Failed to process L-system, `{0}` was thrown.")]
			ExceptionThrownWhileProcessing,

		}

	}
}

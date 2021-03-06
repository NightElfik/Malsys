﻿using System;
using System.Collections.Generic;
using System.Linq;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.Processing.Components;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing {
	/// <summary>
	/// Main class for processing of L-systems.
	/// </summary>
	public class ProcessManager {

		private readonly ICompilersContainer compiler;
		private readonly IEvaluatorsContainer evaluator;
		private readonly IComponentMetadataResolver componentResolver;

		private readonly ProcessConfigurationBuilder configBuilder = new ProcessConfigurationBuilder();


		public ProcessManager(ICompilersContainer compilersContainer, IEvaluatorsContainer evaluatorsContainer, IComponentMetadataResolver componentResolver) {

			compiler = compilersContainer;
			evaluator = evaluatorsContainer;
			this.componentResolver = componentResolver;
		}

		public IComponentMetadataResolver ComponentResolver { get { return componentResolver; } }

		/// <summary>
		/// Compiles and evaluated the input in string.
		/// All messages are logged to give logger.
		/// </summary>
		/// <param name="src">The source code.</param>
		/// <param name="sourcName">The name of the source code for example it can be name of the source file. It will be saved in AST PositionRange classes.</param>
		/// <param name="logger">Logger for logging any messages by compilers or evaluators.</param>
		/// <returns>Evaluated input block. It can be partial or empty if some errors occurred.</returns>
		public InputBlockEvaled CompileAndEvaluateInput(string src, string sourcName, IMessageLogger logger) {

			var inCompiled = compiler.CompileInput(src, sourcName, logger);
			return evaluator.EvaluateInput(inCompiled, evaluator.ExpressionEvaluatorContext, logger);

		}

		/// <summary>
		/// Process all the process statements.
		/// </summary>
		/// <param name="inBlockEvaled">Evaluated input.</param>
		/// <param name="outputProvider">Output provider for storing the outputs.</param>
		/// <param name="logger">Logger that will be supplied to components for logging any messages.</param>
		/// <param name="timeout">Timeout for evaluation.</param>
		public void ProcessInput(InputBlockEvaled inBlockEvaled, IOutputProvider outputProvider, IMessageLogger logger, TimeSpan timeout) {

			foreach (var processStat in inBlockEvaled.ProcessStatements) {

				FSharpMap<string, ConfigurationComponent> components = null;

				try {
					using (var errBlock = logger.StartErrorLoggingBlock()) {
						ProcessConfigurationStatement processConfigStat;
						if (!inBlockEvaled.ProcessConfigurations.TryGetValue(processStat.ProcessConfigName, out processConfigStat)) {
							logger.LogMessage(Message.UndefinedProcessConfig, processStat.AstNode.ProcessConfiNameId.Position, processStat.ProcessConfigName);
							return;
						}

						// create components graph -- graph is same for all processed L-systems with this configuration
						components = configBuilder.CreateComponents(processConfigStat.Components, processConfigStat.Containers,
							processStat.ComponentAssignments, componentResolver, logger);

						if (errBlock.ErrorOccurred) {
							return;
						}

						processInputStatement(processStat, components, processConfigStat.Connections, inBlockEvaled, outputProvider, logger, timeout);
					}
				}
				finally {
					if (components != null) {
						configBuilder.DisposeComponents(components, logger);
					}
				}


			}

		}


		/// <remarks>
		/// Changing of logic in this method may need to change similar logic in ProcessLsystem method of LsystemInLsystemProcessor class.
		/// </remarks>
		private void processInputStatement(ProcessStatementEvaled processStat, FSharpMap<string, ConfigurationComponent> components,
				List<ProcessComponentsConnection> connections, InputBlockEvaled inBlockEvaled, IOutputProvider outputProvider,
				IMessageLogger logger, TimeSpan timeout) {

			IEnumerable<LsystemEvaledParams> lsystemsToProcess;

			lsystemsToProcess = getLsystemsToProcess(processStat, inBlockEvaled, logger);
			if (lsystemsToProcess == null) {
				return;
			}

			// add variables and functions from components that can be called before init to ExpressionEvaluatorContext
			var eec = inBlockEvaled.ExpressionEvaluatorContext;
			eec = configBuilder.AddComponentsGettableVariables(components, eec, true);
			eec = configBuilder.AddComponentsCallableFunctions(components, eec, true);

			var baseResolver = new BaseLsystemResolver(inBlockEvaled.Lsystems);
			var lsysEvaluator = evaluator.ResolveLsystemEvaluator();

			foreach (var lsystem in lsystemsToProcess) {

				try {
					ProcessConfiguration procConfig;
					using (var errBlock = logger.StartErrorLoggingBlock()) {
						// reset must be called before any usage of components
						// also in previous run could occur exception and components can be in bad state
						configBuilder.ResetComponents(components, logger);
						if (errBlock.ErrorOccurred) {
							continue;
						}

						configBuilder.SetLogger(components, logger);

						configBuilder.ConnectComponentsAndCheck(components, connections, logger);

						if (errBlock.ErrorOccurred) {
							continue;
						}

						var lsysEvaled = lsysEvaluator.Evaluate(lsystem, processStat.Arguments, eec, baseResolver, logger);
						if (errBlock.ErrorOccurred) {
							continue;
						}

						// evaluate additional L-system statements from process statement
						lsysEvaluator.EvaluateAdditionalStatements(lsysEvaled, processStat.AdditionalLsystemStatements, logger);
						if (errBlock.ErrorOccurred) {
							continue;
						}

						// Sets settable properties on components.
						configBuilder.SetAndCheckUserSettableProperties(components, lsysEvaled.ComponentValuesAssigns, lsysEvaled.ComponentSymbolsAssigns, logger);

						// Adds gettable variables which can not be get before initialization.
						var newEec = lsysEvaled.ExpressionEvaluatorContext;
						newEec = configBuilder.AddComponentsGettableVariables(components, newEec, false);
						newEec = configBuilder.AddComponentsCallableFunctions(components, newEec, false);

						var context = new ProcessContext(lsysEvaled, outputProvider, inBlockEvaled, evaluator,
							newEec, componentResolver, timeout, components);

						// Initializes components with ExpressionEvaluatorContext.
						configBuilder.InitializeComponents(components, context, logger);

						procConfig = configBuilder.CreateConfiguration(components, logger);
						if (errBlock.ErrorOccurred) {
							continue;
						}
					}

					try {
						procConfig.StarterComponent.Start(procConfig.RequiresMeasure);
					}
					catch (EvalException ex) {
						logger.LogMessage(Message.LsystemEvalFailed, lsystem.Name, ex.GetFullMessage());
					}
					catch (InterpretationException ex) {
						logger.LogMessage(Message.InterpretError, lsystem.Name, ex.Message);
					}
					catch (ComponentException ex) {
						logger.LogMessage(Message.ComponentError, lsystem.Name, ex.Message);
					}
				}
				finally {
					configBuilder.CleanupComponents(components, logger);
				}

			}
		}

		private IEnumerable<LsystemEvaledParams> getLsystemsToProcess(ProcessStatementEvaled processStat, InputBlockEvaled inBlockEvaled, IMessageLogger logger) {

			if (processStat.TargetLsystemName.Length == 0) {
				return inBlockEvaled.Lsystems.Where(x => !x.Value.IsAbstract).Select(x => x.Value);
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


		public enum Message {

			[Message(MessageType.Error, "Undefined process configuration name `{0}`.")]
			UndefinedProcessConfig,
			[Message(MessageType.Error, "Failed to evaluate process statement, L-system `{0}` is not defined.")]
			LsysNotDefined,

			[Message(MessageType.Error, "Evaluation of L-system `{0}` failed. {1}")]
			LsystemEvalFailed,
			[Message(MessageType.Error, "Interpretation error occurred in L-system `{0}`. {1}")]
			InterpretError,
			[Message(MessageType.Error, "Component error occurred in L-system `{0}`. {1}")]
			ComponentError,

		}

	}
}

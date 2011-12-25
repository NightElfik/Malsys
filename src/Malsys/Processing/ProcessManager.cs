using System;
using System.Collections.Generic;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Core;

namespace Malsys.Processing {
	public class ProcessManager {

		private ProcessStatement defaultProcessStatement = new ProcessStatement("",
			ProcessConfigurations.PrintSymbolsConfig.Name, ImmutableList<ProcessComponentAssignment>.Empty);

		private readonly EvaluatorsContainer evaluator = new EvaluatorsContainer();


		public TimeSpan Timeout { get; set; }

		public ProcessManager() {
			Timeout = new TimeSpan(0, 0, 10);
		}


		public SemanticModel.Evaluated.InputBlock CompileAndEvaluateInput(string src, MessageLogger logger) {

			var inCompiled = new CompilersContainer().CompileInput(src, "webInput", logger);

			if (logger.ErrorOcured) {
				return null;
			}

			return evaluator.TryEvaluateInput(inCompiled, logger);
		}


		public void ProcessLsystems(SemanticModel.Evaluated.InputBlock inBlockEvaled, IFilesManager storageManager, MessageLogger logger, IComponentResolver componentResolver) {

			if (inBlockEvaled.Lsystems.Count == 0) {
				logger.LogMessage(Message.NoLsysFoundDumpingConstants);

				new Malsys.Processing.Components.Common.ConstantsDumper().DumpConstants(inBlockEvaled.GlobalConstants, storageManager, logger);
				return;

			}

			foreach (var lsystemKvp in inBlockEvaled.Lsystems) {

				LsystemEvaled lsysEvaled;
				try {
					lsysEvaled = evaluator.EvaluateLsystem(lsystemKvp.Value, ImmutableList<IValue>.Empty,
						inBlockEvaled.GlobalConstants, inBlockEvaled.GlobalFunctions);
				}
				catch (EvalException ex) {
					logger.LogMessage(EvaluatorsContainerExtensions.Message.EvalFailed, ex.GetFullMessage());
					continue;
				}

				var context = new ProcessContext(lsysEvaled, storageManager, inBlockEvaled, evaluator, logger);

				IEnumerable<ProcessStatement> processStatements;
				if(lsysEvaled.ProcessStatements.Length != 0){
					processStatements = lsysEvaled.ProcessStatements;
				}
				else{
					processStatements = new ProcessStatement[]{ defaultProcessStatement };
				}

				var processConfigsMap = inBlockEvaled.ProcessConfigurations;
				processConfigsMap = processConfigsMap.Add(ProcessConfigurations.PrintSymbolsConfig.Name, ProcessConfigurations.PrintSymbolsConfig);
				processConfigsMap = processConfigsMap.Add(ProcessConfigurations.InterpretConfig.Name, ProcessConfigurations.InterpretConfig);

				foreach (var processStat in processStatements) {

					var maybeConfig = processConfigsMap.TryFind(processStat.ProcessConfiName);
					if (OptionModule.IsNone(maybeConfig)) {
						logger.LogError("UnknownProcessConfig", Position.Unknown, "Unknown process configuration `{0}`.".Fmt(processStat.ProcessConfiName));
						continue;
					}
					var configMgr = new ProcessConfigurationManager();

					if (!configMgr.TryBuildConfiguration(maybeConfig.Value, processStat.ComponentAssignments, componentResolver, context, context.Logger)) {
						continue;
					}

					try {
						configMgr.StarterComponent.Start(configMgr.RequiresMeasure, Timeout);
					}
					catch (EvalException ex) {
						logger.LogMessage(EvaluatorsContainerExtensions.Message.EvalFailed, ex.GetFullMessage());
						continue;
					}

					configMgr.ClearComponents();
				}

				storageManager.Cleanup();
			}

		}

		public enum Message {

			[Message(MessageType.Warning, "No L-systems found, dumping constants.")]
			NoLsysFoundDumpingConstants,

		}

	}
}

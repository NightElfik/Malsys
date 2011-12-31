using System;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Core;

namespace Malsys.Processing {
	public class ProcessManager {

		private readonly CompilersContainer compiler;
		private readonly EvaluatorsContainer evaluator;
		private readonly IComponentResolver componentResolver;



		public ProcessManager(CompilersContainer compilersContainer, EvaluatorsContainer evaluatorsContainer, IComponentResolver componentResolver) {

			compiler = compilersContainer;
			evaluator = evaluatorsContainer;
			this.componentResolver = componentResolver;
		}


		public InputBlock CompileAndEvaluateInput(string src, MessageLogger logger) {

			var inCompiled = compiler.CompileInput(src, "webInput", logger);

			if (logger.ErrorOcured) {
				return null;
			}

			return evaluator.TryEvaluateInput(inCompiled, logger);
		}


		public void ProcessLsystems(InputBlock inBlockEvaled, IFilesManager storageManager, MessageLogger logger,
			TimeSpan timeout, bool dumpConstantsIfNoLsystems = true) {

			if (dumpConstantsIfNoLsystems && inBlockEvaled.Lsystems.Count == 0) {
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

				const string defaultConfigName = "SymbolPrinter";
				var procStats = lsysEvaled.ProcessStatements;
				if (procStats.Count == 0 && inBlockEvaled.ProcessConfigurations.ContainsKey(defaultConfigName)) {
					var stat = new Malsys.SemanticModel.Compiled.ProcessStatement(
						null,
						defaultConfigName,
						ImmutableList<Malsys.SemanticModel.Compiled.ProcessComponentAssignment>.Empty);
					procStats = new ImmutableList<SemanticModel.Compiled.ProcessStatement>(stat);
				}

				foreach (var processStat in procStats) {

					var maybeConfig = inBlockEvaled.ProcessConfigurations.TryFind(processStat.ProcessConfiName);
					if (OptionModule.IsNone(maybeConfig)) {
						logger.LogMessage(Message.UndefinedProcessConfig, processStat.AstNode.ProcessConfiNameId.Position, processStat.ProcessConfiName);
						continue;
					}
					var configMgr = new ProcessConfigurationManager();

					if (!configMgr.TryBuildConfiguration(maybeConfig.Value, processStat.ComponentAssignments, componentResolver, context, context.Logger)) {
						continue;
					}

					try {
						configMgr.StarterComponent.Start(configMgr.RequiresMeasure, timeout);
					}
					catch (EvalException ex) {
						logger.LogMessage(EvaluatorsContainerExtensions.Message.EvalFailed, ex.GetFullMessage());
						continue;
					}
					catch (InterpretationException ex) {
						logger.LogMessage(EvaluatorsContainerExtensions.Message.EvalFailed, ex.Message);
						continue;
					}

					configMgr.ClearComponents();
				}

				storageManager.Cleanup();
			}

		}



		public enum Message {

			[Message(MessageType.Error, "Undefined process configuration name `{0}`.")]
			UndefinedProcessConfig,

			[Message(MessageType.Warning, "No L-systems found, dumping constants.")]
			NoLsysFoundDumpingConstants,

		}

	}
}

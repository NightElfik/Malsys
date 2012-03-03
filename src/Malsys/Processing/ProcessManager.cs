using System;
using System.Linq;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Core;
using Malsys.Processing.Output;
using System.Text;

namespace Malsys.Processing {
	public class ProcessManager {

		private readonly CompilersContainer compiler;
		private readonly EvaluatorsContainer evaluator;
		private readonly IComponentResolver componentResolver;

		ProcessConfigurationBuilder processConfigurationBuilder = new ProcessConfigurationBuilder();


		public ProcessManager(CompilersContainer compilersContainer, EvaluatorsContainer evaluatorsContainer, IComponentResolver componentResolver) {

			compiler = compilersContainer;
			evaluator = evaluatorsContainer;
			this.componentResolver = componentResolver;
		}


		public InputBlock CompileAndEvaluateInput(string src, IMessageLogger logger) {

			var inCompiled = compiler.CompileInput(src, "unknownSource", logger);

			if (logger.ErrorOccurred) {
				return null;
			}

			return evaluator.TryEvaluateInput(inCompiled, logger);
		}


		public void ProcessLsystems(InputBlock inBlockEvaled, IOutputProvider outputProvider, IMessageLogger logger,
			TimeSpan timeout, bool dumpConstantsIfNoLsystems = true) {

			if (dumpConstantsIfNoLsystems && inBlockEvaled.Lsystems.Count == 0) {
				logger.LogMessage(Message.NoLsysFoundDumpingConstants);

				new Malsys.Processing.Components.Common.ConstantsDumper().DumpConstants(inBlockEvaled, outputProvider, logger, inBlockEvaled.SourceName);
				return;
			}

			foreach (var lsystemKvp in inBlockEvaled.Lsystems) {

				LsystemEvaled lsysEvaled;
				try {
					lsysEvaled = evaluator.EvaluateLsystem(lsystemKvp.Value, ImmutableList<IValue>.Empty,
						inBlockEvaled.GlobalConstants, inBlockEvaled.GlobalFunctions);
				}
				catch (EvalException ex) {
					logger.LogMessage(IEvaluatorsContainerExtensions.Message.EvalFailed, ex.GetFullMessage());
					continue;
				}

				var context = new ProcessContext(lsysEvaled, outputProvider, inBlockEvaled, evaluator, logger);

				var procStats = getProcessStatements(lsysEvaled, inBlockEvaled, logger);

				foreach (var processStat in procStats) {

					var procConfig = buildProcessConfig(processStat, inBlockEvaled, context, logger);

					if (procConfig == null) {
						continue;
					}

					try {
						procConfig.StarterComponent.Start(procConfig.RequiresMeasure, timeout);
					}
					catch (EvalException ex) {
						logger.LogMessage(IEvaluatorsContainerExtensions.Message.EvalFailed, ex.GetFullMessage());
						continue;
					}
					catch (InterpretationException ex) {
						logger.LogMessage(IEvaluatorsContainerExtensions.Message.EvalFailed, ex.Message);
						continue;
					}

				}

			}

		}


		private ImmutableList<SemanticModel.Compiled.ProcessStatement> getProcessStatements(
				LsystemEvaled lsysEvaled, InputBlock inBlockEvaled, IMessageLogger logger) {

			const string defaultConfigName = "SymbolPrinter";
			if (lsysEvaled.ProcessStatements.Count > 0) {
				return lsysEvaled.ProcessStatements;
			}

			if (inBlockEvaled.ProcessConfigurations.ContainsKey(defaultConfigName)) {
				var stat = new Malsys.SemanticModel.Compiled.ProcessStatement(
					null,
					defaultConfigName,
					ImmutableList<Malsys.SemanticModel.Compiled.ProcessComponentAssignment>.Empty);
				return new ImmutableList<SemanticModel.Compiled.ProcessStatement>(stat);
			}
			else {
				logger.LogMessage(Message.NoProcessStatementsForLsystem, lsysEvaled.Name);
				return ImmutableList<SemanticModel.Compiled.ProcessStatement>.Empty;
			}

		}

		private ProcessConfiguration buildProcessConfig(SemanticModel.Compiled.ProcessStatement processStat,
				InputBlock inBlockEvaled, ProcessContext context, IMessageLogger logger) {

			var maybeConfig = inBlockEvaled.ProcessConfigurations.TryFind(processStat.ProcessConfiName);
			if (OptionModule.IsNone(maybeConfig)) {
				logger.LogMessage(Message.UndefinedProcessConfig, processStat.AstNode.ProcessConfiNameId.Position, processStat.ProcessConfiName);
				return null;
			}

			var config = processConfigurationBuilder.BuildConfiguration(maybeConfig.Value, processStat.ComponentAssignments, componentResolver, context, logger);

			return config;
		}


		public enum Message {

			[Message(MessageType.Error, "Undefined process configuration name `{0}`.")]
			UndefinedProcessConfig,

			[Message(MessageType.Warning, "No L-systems found, dumping constants.")]
			NoLsysFoundDumpingConstants,
			[Message(MessageType.Warning, "No process statements found to process L-system `{0}`.")]
			NoProcessStatementsForLsystem,

		}

	}
}

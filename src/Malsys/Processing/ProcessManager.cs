using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

		ProcessConfigurationBuilder processConfigurationBuilder = new ProcessConfigurationBuilder();


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

				// add variables and functions from components to ExpressionEvaluatorContext
				var eec = inBlockEvaled.ExpressionEvaluatorContext;
				eec = addComponentsGettableVariables(compGraph, eec, true);
				eec = addComponentsCallableFunctions(compGraph, eec);


				foreach (var lsystem in lsystemsToProcess) {

					LsystemEvaled lsysEvaled;
					try {
						lsysEvaled = evaluator.EvaluateLsystem(lsystem, ImmutableList<IValue>.Empty, eec);
					}
					catch (EvalException ex) {
						logger.LogMessage(IEvaluatorsContainerExtensions.Message.EvalFailed, ex.GetFullMessage());
						continue;
					}

					// set settable properties on components
					processConfigurationBuilder.SetAndCheckUserSettableProperties(compGraph, lsysEvaled.ComponentValuesAssigns, lsysEvaled.ComponentSymbolsAssigns, logger);

					// add gettable variables which can not be get before initialization
					var newEec = addComponentsGettableVariables(compGraph, lsysEvaled.ExpressionEvaluatorContext, false);

					var context = new ProcessContext(lsysEvaled, outputProvider, inBlockEvaled, newEec, logger);

					// initialize components with ExpressionEvaluatorContext -- components can call themselves
					var procConfig = processConfigurationBuilder.CreateConfiguration(compGraph, context, logger);
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


		private IEnumerable<LsystemEvaledParams> getLsystemsToProcess(ProcessStatement processStat, InputBlockEvaled inBlockEvaled, IMessageLogger logger) {

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

		private FSharpMap<string, ConfigurationComponent> buildComponentsGraph(ProcessStatement processStat,
				InputBlockEvaled inBlockEvaled, IMessageLogger logger) {

			var maybeConfig = inBlockEvaled.ProcessConfigurations.TryFind(processStat.ProcessConfiName);
			if (OptionModule.IsNone(maybeConfig)) {
				logger.LogMessage(Message.UndefinedProcessConfig, processStat.AstNode.ProcessConfiNameId.Position, processStat.ProcessConfiName);
				return null;
			}

			return processConfigurationBuilder.BuildConfigurationComponentsGraph(maybeConfig.Value, processStat.ComponentAssignments, componentResolver, logger);

		}

		private IExpressionEvaluatorContext addComponentsGettableVariables(FSharpMap<string, ConfigurationComponent> componenets, IExpressionEvaluatorContext eec, bool beforeInit) {

			foreach (var componentKvp in componenets) {

				var component = componentKvp.Value.Component;
				var metadata = componentKvp.Value.Metadata;

				foreach (var gettProp in metadata.GettableProperties.Where(x => x.GettableBeforeInitialiation == beforeInit)) {

					var getFunction = buildComponentVariableCall(gettProp.PropertyInfo, component);

					foreach (var name in gettProp.Names) {
						eec = eec.AddVariable(new VariableInfo(name, getFunction, componentKvp.Value), false);
					}
				}

			}

			return eec;

		}

		private IExpressionEvaluatorContext addComponentsCallableFunctions(FSharpMap<string, ConfigurationComponent> componenets, IExpressionEvaluatorContext eec) {

			foreach (var componentKvp in componenets) {

				var component = componentKvp.Value.Component;
				var metadata = componentKvp.Value.Metadata;

				foreach (var callableFun in metadata.CallableFunctions) {

					var getFunction = buildCallableFunctionCall(callableFun.MethodInfo, component);

					foreach (var name in callableFun.Names) {
						eec = eec.AddFunction(new FunctionInfo(name, callableFun.ParamsCount, getFunction, callableFun.ParamsTypesCyclicPattern, componentKvp.Value), false);
					}
				}

			}

			return eec;

		}

		private Func<IValue> buildComponentVariableCall(PropertyInfo pi, object componentInstance) {

			var instance = Expression.Constant(componentInstance, componentInstance.GetType());
			var call = Expression.Call(instance, pi.GetGetMethod());
			var x = pi.GetGetMethod().GetParameters();
			return Expression.Lambda<Func<IValue>>(call).Compile();

		}

		private Func<IValue[], IExpressionEvaluatorContext, IValue> buildCallableFunctionCall(MethodInfo mi, object componentInstance) {

			var instance = Expression.Constant(componentInstance, componentInstance.GetType());
			var argumentArgs = Expression.Parameter(typeof(IValue[]), "arguments");
			var argumentEec = Expression.Parameter(typeof(IExpressionEvaluatorContext), "expressionEvaluatorContext");
			var call = Expression.Call(instance, mi, argumentArgs, argumentEec);
			return Expression.Lambda<Func<IValue[], IExpressionEvaluatorContext, IValue>>(call, argumentArgs, argumentEec).Compile();

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

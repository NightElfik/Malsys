/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Malsys.Evaluators;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;
using Malsys.Processing.Components.Interpreters;

namespace Malsys.Processing.Components.Common {
	/// <summary>
	///	This is special component for interpreting an L-system symbol as another L-system.
	///	The symbol is processed by newly created component system but interpretation calls are processed with all the
	///	components in the original system.
	/// </summary>
	/// <remarks>
	/// As for now, the inner component system is created from process configuration called "InnerLsystemConfig".
	/// This process configuration must be defined correctly.
	///	Newly created process systems are caches to optimize the performance of repetitive processing.
	/// </remarks>
	/// <name>Inner L-system processor</name>
	/// <group>Special</group>
	public class LsystemInLsystemProcessor : ILsystemInLsystemProcessor {

		protected string DefaultProcessConfigName = "InnerLsystemConfig";

		private const string componentNamePlaceholder = "__this__";


		private readonly ProcessConfigurationBuilder configBuilder = new ProcessConfigurationBuilder();

		private ProcessContext context;

		private Dictionary<Tuple<LsystemEvaledParams, ProcessConfigurationStatement>, Stack<FSharpMap<string, ConfigurationComponent>>> configPool
			= new Dictionary<Tuple<LsystemEvaledParams, ProcessConfigurationStatement>, Stack<FSharpMap<string, ConfigurationComponent>>>();
		private ConfigurationComponent myselfComp;
		private FSharpMap<string, ConfigurationComponent> componentBase;

		private List<FunctionInfo> forwardedFunctions;


		/// <summary>
		/// List of interpreters which should be called by internal interpreter caller.
		/// </summary>
		private List<IInterpreter> interpreters = new List<IInterpreter>();




		public IMessageLogger Logger { get; set; }


		public void Reset() {
			interpreters.Clear();
			context = null;
			myselfComp = null;
			componentBase = null;
			forwardedFunctions = null;

			foreach (var compStack in configPool.Values) {
				foreach (var config in compStack) {
					configBuilder.DisposeComponents(config, Logger);
				}
			}

			configPool.Clear();
		}

		public void Initialize(ProcessContext ctxt) {

			context = ctxt;

			var myslefKvp = context.FindComponent(this);
			if (myslefKvp == null) {
				throw new ComponentException("Could not find components instances.");
			}

			myselfComp = myslefKvp.Value.Value;

			componentBase = MapModule.Empty<string, ConfigurationComponent>()
				.Add(componentNamePlaceholder, myselfComp);


			//forwardedFunctions = context.ExpressionEvaluatorContext.GetAllStoredFunctions()
			//    .Where(x => x.Name.ToLower() == "random")
			//    .ToList();
			forwardedFunctions = context.ExpressionEvaluatorContext.GetAllStoredFunctions().ToList();

		}

		public void Cleanup() { }

		public void Dispose() { }

		/// <summary>
		/// Sets interpreters which will be called by internal caller.
		/// Should be set by parent component in initialization.
		/// </summary>
		public void SetInterpreters(IEnumerable<IInterpreter> ipreters) {
			foreach (var inter in ipreters) {
				// guard to nested hierarchies to prevent adding same interpreter more times
				if (!interpreters.Contains(inter)) {
					interpreters.Add(inter);
				}
			}
		}

		/// <remarks>
		/// The given process configuration must have connections to "virtual" component type of
		/// ILsystemInLsystemProcessor called as "__this__". This component is added to component
		/// graph builder to connect it to new component graph.
		///
		/// Process logic in this method is taken from ProcessInput method of ProcessManager class.
		/// </remarks>
		public void ProcessLsystem(string name, string configName, IValue[] args) {

			LsystemEvaledParams lsystem;
			if (!context.InputData.Lsystems.TryGetValue(name, out lsystem)) {
				throw new ComponentException("Failed to evaluate symbol as lsystem `{0}`. Required L-system is not defined.".Fmt(name));
			}

			ProcessConfigurationStatement configStat;
			if (!context.InputData.ProcessConfigurations.TryGetValue(configName, out configStat)) {
				if (!context.InputData.ProcessConfigurations.TryGetValue(DefaultProcessConfigName, out configStat)) {
					throw new ComponentException("Failed to evaluate symbol as lsystem `{0}`. Required process configuration `{1}` is not defined and default process configuration `{2}` is not defined."
						.Fmt(name, configName, DefaultProcessConfigName));
				}
			}

			FSharpMap<string, ConfigurationComponent> compGraph;

			var configPoolKey = new Tuple<LsystemEvaledParams, ProcessConfigurationStatement>(lsystem, configStat);

			Stack<FSharpMap<string, ConfigurationComponent>> pool;
			if (!configPool.TryGetValue(configPoolKey, out pool) || pool.Count == 0) {
				// create components graph, add this component and interpreter component to it
				using (var errBlock = Logger.StartErrorLoggingBlock()) {
					compGraph = configBuilder.CreateComponents(configStat.Components, configStat.Containers, new ProcessComponentAssignment[0],
						context.ComponentResolver, Logger, componentBase);
					if (errBlock.ErrorOccurred) {
						throw new ComponentException("Failed to evaluate symbol as lsystem `{0}`. Failed to create inner L-system component configuration `{1}`."
							.Fmt(name, configStat.Name));
					}
				}
			}
			else {
				compGraph = pool.Pop();
			}

			// to avoid setting of settable values and initialization of "myself"
			var compGraphOnlyNew = compGraph.Remove(componentNamePlaceholder);

			try {
				ProcessConfiguration procConfig;
				using (var errBlock = Logger.StartErrorLoggingBlock()) {

					// reset must be called before any usage of components
					configBuilder.ResetComponents(compGraphOnlyNew, Logger);
					if (errBlock.ErrorOccurred) {
						throw new ComponentException("Failed to evaluate symbol as lsystem `{0}`. Failed to clean up components."
							.Fmt(name, configStat.Name));
					}

					configBuilder.SetLogger(compGraphOnlyNew, Logger);

					configBuilder.ConnectComponentsAndCheck(compGraph, configStat.Connections, Logger);
					if (errBlock.ErrorOccurred) {
						throw new ComponentException("Failed to evaluate symbol as lsystem `{0}`. Failed to connect inner L-system components `{1}`."
							.Fmt(name, configStat.Name));
					}


					var eec = context.InputData.ExpressionEvaluatorContext;
					// add forwarded functions
					foreach (var fun in forwardedFunctions) {
						eec = eec.AddFunction(fun);
					}
					// add variables and functions from components that can be called before init
					eec = configBuilder.AddComponentsGettableVariables(compGraph, eec, true);
					eec = configBuilder.AddComponentsCallableFunctions(compGraph, eec, true);

					var baseResolver = new BaseLsystemResolver(context.InputData.Lsystems);

					// evaluate L-system
					var lsysEvaled = context.EvaluatorsContainer.ResolveLsystemEvaluator().Evaluate(lsystem, args, eec, baseResolver, Logger);
					if (errBlock.ErrorOccurred) {
						throw new ComponentException("Failed to evaluate symbol as lsystem `{0}`. Failed to evaluate L-system `{1}`."
							.Fmt(name, lsystem.Name));
					}

					// set settable properties on components
					configBuilder.SetAndCheckUserSettableProperties(compGraphOnlyNew, lsysEvaled.ComponentValuesAssigns, lsysEvaled.ComponentSymbolsAssigns, Logger);

					// add gettable variables which can not be get before initialization (do it with full component graph)
					var newEec = lsysEvaled.ExpressionEvaluatorContext;
					newEec = configBuilder.AddComponentsGettableVariables(compGraph, newEec, false);
					newEec = configBuilder.AddComponentsCallableFunctions(compGraph, newEec, false);

					var newContext = new ProcessContext(lsysEvaled, context.OutputProvider, context.InputData, context.EvaluatorsContainer,
						newEec, context.ComponentResolver, context.ProcessingTimeLimit, context.ComponentGraph);


					// add interpreters to caller
					var callerComp = compGraph.Where(x => x.Value.ComponentType == typeof(InterpreterCaller))
						.Select(x => x.Value.Component)
						.FirstOrDefault();

					if (callerComp == null) {
						throw new ComponentException("Failed to find interpreter caller component in inner L-system configuration.");
					}

					foreach (var interp in interpreters) {
						// yes, this looks weird but setter is actually saving interpreters to list
						((InterpreterCaller)callerComp).ExplicitInterpreters = interp;
					}

					// initialize components with ExpressionEvaluatorContext -- components can call themselves
					configBuilder.InitializeComponents(compGraphOnlyNew, newContext, Logger);

					procConfig = configBuilder.CreateConfiguration(compGraph, Logger);
					if (errBlock.ErrorOccurred) {
						throw new ComponentException("Failed to evaluate symbol as lsystem `{0}`.  Failed to initialize inner L-system component configuration `{1}`."
							.Fmt(name, configStat.Name));
					}
				}

				// any exceptions are caught by main processor
				procConfig.StarterComponent.Start(false);  // do not measure internally

			}
			finally {
				// cleanup components before saving them to cache
				configBuilder.CleanupComponents(compGraphOnlyNew, Logger);
				// do not dispose components since they may be reused
			}

			if (pool == null) {
				configPool[configPoolKey] = pool = new Stack<FSharpMap<string, ConfigurationComponent>>();
			}

			// save components graph to pool
			pool.Push(compGraph);

		}


	}
}

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

namespace Malsys.Processing.Components.Common {
	/// <summary>
	///	This is special component for interpreting L-system symbol as another L-system.
	///	Inner component systems used to process system must be defined under the name "InnerLsystemConfig".
	///	It caches component systems for processing inner L-system to optimize speed of repetitive processing.
	/// </summary>
	/// <name>Inner L-system processor</name>
	/// <group>Special</group>
	public class LsystemInLsystemProcessor : ILsystemInLsystemProcessor {

		protected string DefaultProcessConfigName = "InnerLsystemConfig";


		private readonly ProcessConfigurationBuilder configBuilder = new ProcessConfigurationBuilder();

		private ProcessContext ctxt;

		private Dictionary<Tuple<LsystemEvaledParams, ProcessConfigurationStatement>, Stack<FSharpMap<string, ConfigurationComponent>>> configPool
			= new Dictionary<Tuple<LsystemEvaledParams, ProcessConfigurationStatement>, Stack<FSharpMap<string, ConfigurationComponent>>>();
		private ConfigurationComponent myselfComp;
		private FSharpMap<string, ConfigurationComponent> componentBase;

		private List<FunctionInfo> forwardedFunctions;


		public IMessageLogger Logger { get; set; }


		public void Initialize(ProcessContext context) {

			ctxt = context;

			var myslefKvp = ctxt.FindComponent(this);
			if (myslefKvp == null) {
				throw new ComponentException("Could not find components instances.");
			}

			myselfComp = myslefKvp.Value.Value;

			componentBase = MapModule.Empty<string, ConfigurationComponent>()
				.Add(myselfComp.Name, myselfComp);


			forwardedFunctions = ctxt.ExpressionEvaluatorContext.GetAllStoredFunctions()
				.Where(x => x.Name.ToLower() == "random")
				.ToList();

		}

		public void Cleanup() {
			ctxt = null;
			myselfComp = null;
			componentBase = null;
			forwardedFunctions = null;
			configPool.Clear();
		}



		/// <remarks>
		/// The given process configuration must have connections to "virtual" component type of
		/// ILsystemInLsystemProcessor called with same name as this component. This component is added to component
		/// graph builder to connect it to new component graph.
		/// </remarks>
		public void ProcessLsystem(string name, string configName, IValue[] args) {

			LsystemEvaledParams lsystem;
			if (!ctxt.InputData.Lsystems.TryGetValue(name, out lsystem)) {
				throw new ComponentException("Failed to evaluate symbol as lsystem `{0}`. Required L-system is not defined.".Fmt(name));
			}

			ProcessConfigurationStatement configStat;
			if (!ctxt.InputData.ProcessConfigurations.TryGetValue(configName, out configStat)) {
				if (!ctxt.InputData.ProcessConfigurations.TryGetValue(DefaultProcessConfigName, out configStat)) {
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
						ctxt.ComponentResolver, Logger, componentBase);
					if (errBlock.ErrorOccurred) {
						throw new ComponentException("Failed to evaluate symbol as lsystem `{0}`. Failed to create inner L-system component configuration `{1}`."
							.Fmt(name, configStat.Name));
					}
				}
			}
			else {
				compGraph = pool.Pop();
			}

			// to avoid setting of settable values and initialization of myself and interpreter
			// they are already connected so we can remove them from this list
			var compGraphOnlyNew = compGraph.Remove(myselfComp.Name);
			configBuilder.SetLogger(compGraphOnlyNew, Logger);

			ProcessConfiguration procConfig;
			using (var errBlock = Logger.StartErrorLoggingBlock()) {

				// cleanup must be called before any usage of components
				configBuilder.CleanupComponents(compGraphOnlyNew, Logger);
				if (errBlock.ErrorOccurred) {
					throw new ComponentException("Failed to evaluate symbol as lsystem `{0}`. Failed to clean up components."
						.Fmt(name, configStat.Name));
				}

				configBuilder.ConnectComponentsAndCheck(compGraph, configStat.Connections, Logger);
				if (errBlock.ErrorOccurred) {
					throw new ComponentException("Failed to evaluate symbol as lsystem `{0}`. Failed to connect inner L-system components `{1}`."
						.Fmt(name, configStat.Name));
				}


				var eec = ctxt.InputData.ExpressionEvaluatorContext;
				// add forwarded functions
				foreach (var fun in forwardedFunctions) {
					eec = eec.AddFunction(fun);
				}
				// add variables and functions from components that can be called before init
				eec = configBuilder.AddComponentsGettableVariables(compGraph, eec, true);
				eec = configBuilder.AddComponentsCallableFunctions(compGraph, eec, true);

				var baseResolver = new BaseLsystemResolver(ctxt.InputData.Lsystems);

				// evaluate L-system
				var lsysEvaled = ctxt.EvaluatorsContainer.ResolveLsystemEvaluator().Evaluate(lsystem, args, eec, baseResolver, Logger);
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

				var newContext = new ProcessContext(lsysEvaled, ctxt.OutputProvider, ctxt.InputData, ctxt.EvaluatorsContainer,
					newEec, ctxt.ComponentResolver, ctxt.ProcessingTimeLimit, ctxt.ComponentGraph);

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


			// cleanup components before saving them to cache
			configBuilder.CleanupComponents(compGraphOnlyNew, Logger);

			if (pool == null) {
				configPool[configPoolKey] = pool = new Stack<FSharpMap<string, ConfigurationComponent>>();
			}

			// save components graph to pool
			pool.Push(compGraph);

		}


	}
}

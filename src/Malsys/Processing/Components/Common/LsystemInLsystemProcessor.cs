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
	///	It caches process components for processing inner L-system to optimize speed of processing.
	/// </summary>
	/// <name>Inner L-system processor</name>
	/// <group>Special</group>
	public class LsystemInLsystemProcessor : ILsystemInLsystemProcessor {

		public const string DefaultProcessConfigName = "InnerLsystemConfig";

		private readonly ProcessConfigurationBuilder configBuilder = new ProcessConfigurationBuilder();


		private IInterpreter interpreter;
		private ProcessContext ctxt;

		private Dictionary<Tuple<LsystemEvaledParams, ProcessConfigurationStatement>, Stack<FSharpMap<string, ConfigurationComponent>>> configPool;
		private ConfigurationComponent myselfComp;
		private ConfigurationComponent interpreterComp;
		private FSharpMap<string, ConfigurationComponent> componentBase;

		/// <summary>
		/// Interpreter of main L-system.
		/// All symbols from inner L-system should be processed with same interpreter as main L-system.
		/// This connection is set to optional due to technical reasons but must be set to same interpreter component as
		/// main L-system. Otherwise Exception is thrown.
		/// </summary>
		/// <remarks>
		/// This connection is set to optional because this component is part of inner configuration when ProcessManager
		/// is building inner configuration there is no interpreter component to no override main interpreter.
		/// To avoid errors of unconnected connection this is optional.
		/// Connection is set after creating inner process configuration from outside.
		/// </remarks>
		[UserConnectable(IsOptional = true)]
		public IInterpreter Interpreter {
			set { interpreter = value; }
		}


		#region IProcessComponent Members

		public void Initialize(ProcessContext context) {

			ctxt = context;
			configPool = new Dictionary<Tuple<LsystemEvaledParams, ProcessConfigurationStatement>, Stack<FSharpMap<string, ConfigurationComponent>>>();

			if (interpreter == null) {
				throw new ComponentException("Interpreter is not set.");
			}

			myselfComp = ctxt.ComponentGraph.Where(x => x.Value.Component == this).Single().Value;
			interpreterComp = ctxt.ComponentGraph.Where(x => x.Value.Component == interpreter).Single().Value;

			componentBase = MapModule.Empty<string, ConfigurationComponent>()
				.Add(myselfComp.Name, myselfComp)
				.Add(interpreterComp.Name, interpreterComp);

		}

		public void Cleanup() {
			ctxt = null;
		}

		#endregion


		/// <remarks>
		/// The given process configuration must have connections to "virtual" components type of
		/// ILsystemInLsystemProcessor and IInterpreter called with same name as this component
		/// and given interpreter. These two components are added to component graph builder to connect them to new
		/// component graph.
		/// </remarks>
		public void ProcessLsystem(string name, string configName, IValue[] args) {

			LsystemEvaledParams lsystem;
			if (!ctxt.InputData.Lsystems.TryGetValue(name, out lsystem)) {
				ctxt.Logger.LogMessage(Message.LsystemNotFound, name);
				return;
			}

			ProcessConfigurationStatement configStat;
			if (!ctxt.InputData.ProcessConfigurations.TryGetValue(configName, out configStat)) {
				if (!ctxt.InputData.ProcessConfigurations.TryGetValue(DefaultProcessConfigName, out configStat)) {
					ctxt.Logger.LogMessage(Message.NoProcessConfig, name, configName, DefaultProcessConfigName);
					return;
				}
			}

			FSharpMap<string, ConfigurationComponent> compGraph;

			var configPoolKey = new Tuple<LsystemEvaledParams, ProcessConfigurationStatement>(lsystem, configStat);

			Stack<FSharpMap<string, ConfigurationComponent>> pool;
			if (!configPool.TryGetValue(configPoolKey, out pool) || pool.Count == 0) {
				// create components graph, add this component and interpreter component to it
				compGraph = configBuilder.BuildConfigurationComponentsGraph(configStat, new ProcessComponentAssignment[0], ctxt.ComponentResolver, ctxt.Logger, componentBase);
				if (compGraph == null) {
					return;
				}
			}
			else {
				compGraph = pool.Pop();
			}


			// to avoid setting of settable values and initialization of myself and interpreter, but they are already connected
			var compGraphOnlyNew = compGraph.Remove(myselfComp.Name).Remove(interpreterComp.Name);

			// add variables and functions from components that can be called before init to ExpressionEvaluatorContext
			var eec = ctxt.InputData.ExpressionEvaluatorContext;
			eec = configBuilder.AddComponentsGettableVariables(compGraph, eec, true);
			eec = configBuilder.AddComponentsCallableFunctions(compGraph, eec, true);

			var baseResolver = new BaseLsystemResolver(ctxt.InputData.Lsystems);

			ProcessConfiguration procConfig;
			using (var errBlock = ctxt.Logger.StartErrorLoggingBlock()) {
				// evaluate L-system
				var lsysEvaled = ctxt.EvaluatorsContainer.TryEvaluateLsystem(lsystem, args, ctxt.ExpressionEvaluatorContext, baseResolver, ctxt.Logger);
				if (errBlock.ErrorOccurred) {
					return;
				}


				// set settable properties on components
				configBuilder.SetAndCheckUserSettableProperties(compGraphOnlyNew, lsysEvaled.ComponentValuesAssigns, lsysEvaled.ComponentSymbolsAssigns, ctxt.Logger);

				// add gettable variables which can not be get before initialization (do it with full component graph)
				var newEec = lsysEvaled.ExpressionEvaluatorContext;
				newEec = configBuilder.AddComponentsGettableVariables(compGraph, newEec, false);
				newEec = configBuilder.AddComponentsCallableFunctions(compGraph, newEec, false);

				var newContext = new ProcessContext(lsysEvaled, ctxt.OutputProvider, ctxt.InputData, ctxt.EvaluatorsContainer,
					newEec, ctxt.ComponentResolver, ctxt.ProcessingTimeLimit, ctxt.ComponentGraph, ctxt.Logger);

				// initialize components with ExpressionEvaluatorContext -- components can call themselves
				configBuilder.InitializeComponents(compGraphOnlyNew, newContext, ctxt.Logger);

				procConfig = configBuilder.CreateConfiguration(compGraph, ctxt.Logger);
				if (errBlock.ErrorOccurred) {
					return;
				}
			}

			try {
				procConfig.StarterComponent.Start(false);  // do not measure internally
			}
			catch (EvalException ex) {
				ctxt.Logger.LogMessage(Message.LsystemEvalFailed, name, ex.GetFullMessage());
			}
			catch (InterpretationException ex) {
				ctxt.Logger.LogMessage(Message.InterpretError, name, ex.Message);
			}

			configBuilder.CleanupComponents(compGraphOnlyNew, ctxt.Logger);

			if (pool == null) {
				configPool[configPoolKey] = pool = new Stack<FSharpMap<string, ConfigurationComponent>>();
			}

			pool.Push(compGraph);

		}


		public enum Message {

			[Message(MessageType.Error, "Failed to evaluate symbol as lsystem `{0}`. Required L-system is not defined.")]
			LsystemNotFound,
			[Message(MessageType.Error, "Failed to evaluate symbol as lsystem `{0}`. Required process configuration `{1}` is not defined and default process configuration `{2}` is not defined.")]
			NoProcessConfig,
			[Message(MessageType.Error, "Failed to evaluate symbol as lsystem `{0}`. {1}")]
			LsystemEvalFailed,
			[Message(MessageType.Error, "Failed to evaluate symbol as lsystem `{0}`. Interpretation error occurred. {1}")]
			InterpretError,

		}

	}
}

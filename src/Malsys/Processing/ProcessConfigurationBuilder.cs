using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Malsys.Evaluators;
using Malsys.Processing.Components;
using Malsys.Reflection.Components;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing {
	/// <summary>
	/// Class for building process configuration.
	/// </summary>
	/// <remarks>
	/// In order to build process configuration correctly you should call these methods if following order:
	/// <c>BuildConfigurationComponentsGraph</c>, <c>AddComponentsGettableVariables</c> and <c>AddComponentsCallableFunctions</c>
	/// with beforeInit parameter set to true, <c>SetAndCheckUserSettableProperties</c>,
	/// <c>InitializeComponents</c>,  <c>AddComponentsGettableVariables</c> and <c>AddComponentsCallableFunctions</c>
	/// with beforeInit parameter set to false and finally <c>CreateConfiguration</c> respectively.
	/// Methods are separated to enable getting variables or calling functions while evaluating L-system
	/// and initializing components (before init).
	/// Do not forget to call <c>CleanupComponents</c> at the end of processing.
	/// </remarks>
	public class ProcessConfigurationBuilder {

		/// <summary>
		/// Crates instances of all needed components and connects all connections.
		/// </summary>
		/// <param name="processConfigStat">The "build" plan.</param>
		/// <param name="componentsAssigns">Assignments of components to containers. Can be empty list because every container has default value.</param>
		/// <param name="componentResolver">Component metadata resolver. Used to resolve component metadata from string from Process Configuration.</param>
		/// <param name="logger">Logger for logging any failures during process.</param>
		/// <param name="componentsBase">Already created components.</param>
		/// <returns>F# map of connected components or null if error occurred.</returns>
		public FSharpMap<string, ConfigurationComponent> BuildConfigurationComponentsGraph(ProcessConfigurationStatement processConfigStat,
				IEnumerable<ProcessComponentAssignment> componentsAssigns, IComponentMetadataResolver componentResolver, IMessageLogger logger,
				FSharpMap<string, ConfigurationComponent> componentsBase = null) {

			using (var errBlock = logger.StartErrorLoggingBlock()) {

				FSharpMap<string, ConfigurationComponent> components = componentsBase ?? MapModule.Empty<string, ConfigurationComponent>();
				var compAssignsDict = componentsAssigns.ToDictionary(ca => ca.ContainerName, ca => ca.ComponentTypeName);
				var usedCompAssigns = new System.Collections.Generic.HashSet<string>();


				// components
				foreach (var procComp in processConfigStat.Components) {
					var comp = createComponent(procComp.Name, procComp.TypeName, componentResolver, logger);
					if (comp != null) {
						components = components.Add(comp.Name, comp);
					}
				}

				// containers
				foreach (var procCont in processConfigStat.Containers) {

					string contCompTypeName;
					if (compAssignsDict.TryGetValue(procCont.Name, out contCompTypeName)) {
						usedCompAssigns.Add(procCont.Name);
					}
					else {
						contCompTypeName = procCont.DefaultTypeName;
					}

					var comp = createContaineredComponent(procCont.Name, procCont.TypeName, contCompTypeName, componentResolver, logger);
					if (comp != null) {
						components = components.Add(comp.Name, comp);
					}
				}

				if (errBlock.ErrorOccurred) {
					// return early if some error occurred to prevent weird errors
					return null;
				}

				connectAndCheck(components, processConfigStat.Connections, logger);
				if (errBlock.ErrorOccurred) {
					return null;
				}

				// check if all component assignments were used or log warning if not
				foreach (var compAssignKvp in compAssignsDict) {
					if (!usedCompAssigns.Contains(compAssignKvp.Key)) {
						logger.LogMessage(Message.ComponentAssignNotUsed, compAssignKvp.Value, compAssignKvp.Key);
					}
				}

				return components;
			}
		}

		/// <summary>
		/// Sets and checks all settable properties in all given components.
		/// </summary>
		/// <param name="components">F# list of components to process.</param>
		/// <param name="valueAssigns">Values to assign to settable properties.</param>
		/// <param name="symbolsAssigns">Symbols to assign to symbol settable properties.</param>
		/// <param name="logger">Logger for logging any failures during process.</param>
		public void SetAndCheckUserSettableProperties(FSharpMap<string, ConfigurationComponent> components,
				FSharpMap<string, IValue> valueAssigns, FSharpMap<string, ImmutableList<Symbol<IValue>>> symbolsAssigns, IMessageLogger logger) {

			var setValues = new System.Collections.Generic.HashSet<string>();
			var setSymbols = new System.Collections.Generic.HashSet<string>();

			foreach (var kvp in components) {

				var comp = kvp.Value;

				// settable properties
				foreach (var settPropMeta in comp.Metadata.SettableProperties) {
					string name = trySetSettableProperty(comp, settPropMeta, valueAssigns, logger);
					if (name != null) {
						setValues.Add(name);
					}
				}

				// settable symbol properties
				foreach (var settSymPropMeta in comp.Metadata.SettableSymbolsProperties) {
					string name = trySetSettableSymbolProperty(comp, settSymPropMeta, symbolsAssigns, logger);
					if (name != null) {
						setSymbols.Add(name);
					}
				}

			}

			foreach (var valAssign in valueAssigns) {
				if (!setValues.Contains(valAssign.Key)) {
					logger.LogMessage(Message.ComponentValueAssignNotUsed, valAssign.Key);
				}
			}

			foreach (var symAssign in symbolsAssigns) {
				if (!setSymbols.Contains(symAssign.Key)) {
					logger.LogMessage(Message.ComponentSymbolAssignNotUsed, symAssign.Key);
				}
			}

		}


		/// <summary>
		/// Initializes given components with given context.
		/// </summary>
		public void InitializeComponents(FSharpMap<string, ConfigurationComponent> components, ProcessContext ctxt, IMessageLogger logger) {

			foreach (var kvp in components) {

				var comp = kvp.Value;

				try {
					comp.Component.Initialize(ctxt);
				}
				catch (ComponentException ex) {
					logger.LogMessage(Message.ComponentInitializationError, comp.Name, comp.ComponentType.FullName, ex.Message);
					continue;
				}
#if !DEBUG  // to not catch all exceptions while debugging and allow debugger to catch them
				catch (Exception ex) {
					logger.LogMessage(Message.ComponentInitializationException, ex.GetType().Name, comp.Name, comp.ComponentType.FullName);
					continue;
				}
#endif

			}
		}

		/// <summary>
		/// Checks if measuring pass is required by any component.
		/// From these information returns ProcessConfiguration.
		/// This is last step in creation of ProcessConfiguration components must be already initialized.
		/// </summary>
		/// <param name="configurationComponents">Component graph from BuildConfigurationComponentsGraph method.</param>
		/// <param name="ctxt">Process context with which will be components initialized.</param>
		/// <param name="logger">Logger for logging any failures during process.</param>
		/// <returns>Valid ProcessConfiguration or null if error occurred.</returns>
		public ProcessConfiguration CreateConfiguration(FSharpMap<string, ConfigurationComponent> configurationComponents, IMessageLogger logger) {

			bool requiresMeasure = false;
			foreach (var kvp in configurationComponents) {
				if (kvp.Value.Component is IProcessComponent) {
					requiresMeasure |= ((IProcessComponent)kvp.Value.Component).RequiresMeasure;
				}
			}

			var starter = getStarterComponent(configurationComponents, logger);
			if (starter == null) {
				return null;
			}

			return new ProcessConfiguration(configurationComponents, requiresMeasure, starter);
		}

		/// <summary>
		/// Calls Cleanup method on all given components.
		/// </summary>
		public void CleanupComponents(FSharpMap<string, ConfigurationComponent> components, IMessageLogger logger) {

			foreach (var kvp in components) {

				var comp = kvp.Value;

				try {
					comp.Component.Cleanup();
				}
				catch (ComponentException ex) {
					logger.LogMessage(Message.ComponentCleanupError, comp.Name, comp.ComponentType.FullName, ex.Message);
					continue;
				}
#if !DEBUG  // to not catch all exceptions while debugging and allow debugger to catch them
				catch (Exception ex) {
					logger.LogMessage(Message.ComponentCleanupException, ex.GetType().Name, comp.Name, comp.ComponentType.FullName);
					continue;
				}
#endif


			}
		}



		public IExpressionEvaluatorContext AddComponentsGettableVariables(FSharpMap<string, ConfigurationComponent> componenets, IExpressionEvaluatorContext eec, bool beforeInit) {

			foreach (var componentKvp in componenets) {

				var component = componentKvp.Value.Component;
				var metadata = componentKvp.Value.Metadata;

				foreach (var gettProp in metadata.GettableProperties.Where(x => x.IsGettableBeforeInitialiation == beforeInit)) {

					var getFunction = buildComponentVariableCall(gettProp.PropertyInfo, component);

					foreach (var name in gettProp.Names) {
						eec = eec.AddVariable(new VariableInfo(name, getFunction, componentKvp.Value), false);
					}
				}

			}

			return eec;

		}

		public IExpressionEvaluatorContext AddComponentsCallableFunctions(FSharpMap<string, ConfigurationComponent> componenets, IExpressionEvaluatorContext eec, bool beforeInit) {

			foreach (var componentKvp in componenets) {

				var component = componentKvp.Value.Component;
				var metadata = componentKvp.Value.Metadata;

				foreach (var callableFun in metadata.CallableFunctions.Where(x => x.IsGettableBeforeInitialiation == beforeInit)) {

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



		private IProcessStarter getStarterComponent(FSharpMap<string, ConfigurationComponent> components, IMessageLogger logger) {

			IProcessStarter starterComponent = null;

			foreach (var kvp in components) {
				if (kvp.Value.Component is IProcessStarter) {
					if (starterComponent != null) {
						logger.LogMessage(Message.MoreStartComponents, typeof(IProcessStarter).Name);
					}
					starterComponent = (IProcessStarter)kvp.Value.Component;
				}
			}

			if (starterComponent == null) {
				logger.LogMessage(Message.NoStartComponent, typeof(IProcessStarter).Name);
			}

			return starterComponent;
		}

		private ConfigurationComponent createComponent(string compName, string compTypeName, IComponentMetadataResolver componentResolver, IMessageLogger logger) {

			var metadata = componentResolver.ResolveComponentMetadata(compTypeName, logger);
			if (metadata == null) {
				logger.LogMessage(Message.ComponentResolveError, compName, compTypeName);
				return null;
			}

			IComponent componentInstance;

			try {
				componentInstance = (IComponent)(metadata.HasCtorWithMessageLogger
					? metadata.ComponentConstructor.Invoke(new object[] { logger })
					: metadata.ComponentConstructor.Invoke(null));
			}
			catch (Exception ex) {
				logger.LogMessage(Message.ComponentCtorException, ex.GetType().Name, compName, metadata.ComponentType.FullName);
				return null;
			}

			return new ConfigurationComponent(compName, componentInstance, metadata);

		}

		private ConfigurationComponent createContaineredComponent(string compName, string contTypeName, string compTypeName,
				IComponentMetadataResolver componentResolver, IMessageLogger logger) {

			var contType = componentResolver.ResolveComponentType(contTypeName);
			if (contType == null) {
				logger.LogMessage(Message.ContainerResolveError, compName, contTypeName);
				return null;
			}

			var comp = createComponent(compName, compTypeName, componentResolver, logger);
			if (comp == null) {
				return null;
			}

			if (!contType.IsAssignableFrom(comp.ComponentType)) {
				logger.LogMessage(Message.ComponentDontFitContainer, compName, contType.FullName, comp.ComponentType.FullName);
				return null;
			}

			return comp;
		}

		/// <remarks>
		/// Now it is possible to connect a connection, that is not visible (illegal) from configuration:
		/// Component in container can use properties, that are not declared in its container.
		/// </remarks>
		private void connectAndCheck(FSharpMap<string, ConfigurationComponent> components, IEnumerable<ProcessComponentsConnection> connections, IMessageLogger logger) {


			var usedConnections = new System.Collections.Generic.HashSet<ProcessComponentsConnection>();

			foreach (var compKvp in components) {

				var component = compKvp.Value;
				var aviableComponentConns = connections.Where(c => c.TargetName == component.Name).ToList();

				foreach (var connProp in component.Metadata.ConnectableProperties) {

					var conn = aviableComponentConns.Where(c => connProp.Names.Contains(c.TargetInputName)).ToList();

					if (conn.Count == 0) {
						if (!connProp.IsOptional) {
							logger.LogMessage(Message.UnsetMandatoryConnection, component.Name, connProp.Names[0], component.ComponentType.FullName);
						}
						continue;
					}
					else if (!connProp.AllowMultiple && conn.Count > 1) {
						logger.LogMessage(Message.MoreThanOneConnection, component.Name, conn[0].TargetInputName, component.ComponentType.FullName);
					}

					foreach (var c in conn) {
						connectComponents(components, c, logger);
						usedConnections.Add(c);
					}

				}
			}

			foreach (var conn in connections) {
				if (!usedConnections.Contains(conn)) {
					logger.LogMessage(Message.InvalidConnection, conn.SourceName, conn.TargetName, conn.TargetInputName);
				}
			}

		}

		private void connectComponents(FSharpMap<string, ConfigurationComponent> components, ProcessComponentsConnection conn, IMessageLogger logger) {

			ConfigurationComponent srcComp;
			if (!components.TryGetValue(conn.SourceName, out srcComp)) {
				// this is checked by compiler but only on non-virtual connections, moreover configuration may not be from compiler
				logger.LogMessage(Message.FailedToConnect, conn.SourceName, conn.TargetName, conn.TargetInputName, conn.TargetName);
				return;
			}

			ConfigurationComponent destComp;
			if (!components.TryGetValue(conn.TargetName, out destComp)) {
				// method calling this method is iterating over existing components so this should not happen
				// this is checked by compiler but only on non-virtual connections, moreover configuration may not be from compiler
				logger.LogMessage(Message.FailedToConnect, conn.SourceName, conn.TargetName, conn.TargetInputName, conn.TargetInputName);
				return;
			}

			ComponentConnectablePropertyMetadata connProp;
			if (!destComp.Metadata.TryGetConnectableProperty(conn.TargetInputName, out connProp)) {
				// method calling this method is iterating over existing properties so this should not happen
				logger.LogMessage(Message.FailedToConnectPropertyDontExist, conn.SourceName, conn.TargetName, conn.TargetInputName);
				return;
			}

			if (!connProp.PropertyType.IsAssignableFrom(srcComp.ComponentType)) {
				logger.LogMessage(Message.FailedToConnectNotAssignable, conn.SourceName, conn.TargetName, conn.TargetInputName);
				return;
			}

			try {
				connProp.PropertyInfo.SetValue(destComp.Component, srcComp.Component, null);
			}
			catch (TargetInvocationException ex) {
				if (ex.InnerException is InvalidConnectedComponentException) {
					logger.LogMessage(Message.FailedToConnectInvalidValue, conn.SourceName, conn.TargetName, conn.TargetInputName, ex.InnerException.Message);
				}
				else {
					throw;
				}
			}
		}


		/// <returns>The name of successfully set variable or null.</returns>
		private string trySetSettableProperty(ConfigurationComponent comp, ComponentSettablePropertyMetadata settPropMeta,
				FSharpMap<string, IValue> valueAssigns, IMessageLogger logger) {

			string setName = null;
			bool valueSet = false;

			foreach (var name in settPropMeta.Names) {
				IValue constVal;
				if (valueAssigns.TryGetValue(name, out constVal)) {

					if (settPropMeta.PropertyType.IsAssignableFrom(constVal.GetType())) {
						valueSet = trySetPropertyValue(settPropMeta.PropertyInfo, comp, constVal, name, logger);
					}
					else {
						logger.LogMessage(Message.FailedToSetPropertyValueIncompatibleTypes,
							name, comp.Name, comp.ComponentType.FullName, constVal.Type.ToTypeString(), settPropMeta.ExpressionValueType.ToTypeString());
					}

					setName = name;
					break;

				}
			}

			if (settPropMeta.IsMandatory && !valueSet) {
				logger.LogMessage(Message.UnsetMandatoryProperty, settPropMeta.Names[0], comp.Name, comp.ComponentType.FullName);
			}

			return setName;
		}

		/// <returns>The name of successfully set variable or null.</returns>
		private string trySetSettableSymbolProperty(ConfigurationComponent comp, ComponentSettableSybolsPropertyMetadata settSymPropMeta,
				FSharpMap<string, ImmutableList<Symbol<IValue>>> symbolsAssigns, IMessageLogger logger) {

			string setName = null;
			bool valueSet = false;

			foreach (var name in settSymPropMeta.Names) {
				ImmutableList<Symbol<IValue>> symbolsVal;
				if (symbolsAssigns.TryGetValue(name, out symbolsVal)) {
					valueSet = trySetPropertyValue(settSymPropMeta.PropertyInfo, comp, symbolsVal, name, logger);
					setName = name;
					break;

				}
			}

			if (settSymPropMeta.IsMandatory && !valueSet) {
				logger.LogMessage(Message.UnsetMandatoryProperty, settSymPropMeta.Names[0], comp.Name, comp.ComponentType.FullName);
			}

			return setName;
		}

		private bool trySetPropertyValue(PropertyInfo pi, ConfigurationComponent comp, object value, string propName, IMessageLogger logger) {

			try {
				pi.SetValue(comp.Component, value, null);
				return true;
			}
			catch (TargetInvocationException ex) {
				if (ex.InnerException is InvalidUserValueException) {
					logger.LogMessage(Message.FailedToSetPropertyValue, propName, comp.Name, comp.ComponentType.FullName, ex.InnerException.Message);
				}
				else {
					throw;
				}
			}

			return false;
		}


		public enum Message {

			[Message(MessageType.Error, "Configuration do not have starting component (component implementing `{0}` interface).")]
			NoStartComponent,
			[Message(MessageType.Error, "Configuration have more than one starting component (component implementing `{0}` interface).")]
			MoreStartComponents,
			[Message(MessageType.Error, "Unset mandatory connection `{0}.{1}` of `{2}`.")]
			UnsetMandatoryConnection,
			[Message(MessageType.Error, "More than one component is connected to `{0}.{1}` of `{2}` which do not support multiple connections.")]
			MoreThanOneConnection,
			[Message(MessageType.Error, "Unset mandatory property `{0}` of `{1}` (`{2}`).")]
			UnsetMandatoryProperty,
			[Message(MessageType.Error, "Failed to resolve component `{0}` with type `{1}`.")]
			ComponentResolveError,
			[Message(MessageType.Error, "Failed to resolve container `{0}` with type `{1}`.")]
			ContainerResolveError,
			[Message(MessageType.Error, "`{0}` thrown on creation of component `{1}` with type `{2}`.")]
			ComponentCtorException,
			[Message(MessageType.Error, "Container `{0}` of type `{1}` in not compatible with component of type `{2}`.")]
			ComponentDontFitContainer,
			[Message(MessageType.Error, "Failed to connect `{0}` to `{1}.{2}`, `{3}` not found.")]
			FailedToConnect,
			[Message(MessageType.Error, "Failed to connect `{0}` to `{1}.{2}`, public property `{2}` on `{1}` does not exist.")]
			FailedToConnectPropertyDontExist,
			[Message(MessageType.Error, "Failed to connect `{0}` to `{1}.{2}`, `{0}` is not assignable to `{1}.{2}`.")]
			FailedToConnectNotAssignable,
			[Message(MessageType.Error, "Failed to connect `{0}` to `{1}.{2}`. {3}")]
			FailedToConnectInvalidValue,
			[Message(MessageType.Error, "Failed initialize component `{0}` (`{1}`). {2}")]
			ComponentInitializationError,
			[Message(MessageType.Error, "`{0}` thrown on initialization of component `{1}` (`{2}`).")]
			ComponentInitializationException,
			[Message(MessageType.Error, "Failed clean up component `{0}` (`{1}`). {2}")]
			ComponentCleanupError,
			[Message(MessageType.Error, "`{0}` thrown on cleanup of component `{1}` (`{2}`).")]
			ComponentCleanupException,
			[Message(MessageType.Error, "Invalid connection rule connecting `{0}` to `{1}`.`{2}`.")]
			InvalidConnection,


			[Message(MessageType.Warning, "Expected constant as value of property `{0}` of `{1}` (`{2}`).")]
			ExpectedConstantAsValue,
			[Message(MessageType.Warning, "Expected array as value of property `{0}` of `{1}` (`{2}`).")]
			ExpectedArrayAsValue,
			[Message(MessageType.Warning, "Expected symbols list as value of property `{0}` of `{1}` (`{2}`).")]
			ExpectedSymbolListAsValue,
			[Message(MessageType.Warning, "Expected constant or array as value of property `{0}` of `{1}` (`{2}`).")]
			ExpectedIValueAsValue,
			[Message(MessageType.Warning, "Failed to set value of property `{0}` of component `{1}` of type `{2}`. {3}")]
			FailedToSetPropertyValue,
			[Message(MessageType.Warning, "Failed to set value of property `{0}` of component `{1}` of type `{2}`. Value `{3}` is not assignable to `{4}`.")]
			FailedToSetPropertyValueIncompatibleTypes,
			[Message(MessageType.Warning, "Component assign of component `{0}` to container `{1}` is not used.")]
			ComponentAssignNotUsed,
			[Message(MessageType.Warning, "Component value assignment `{0}` not used. No component to assign to.")]
			ComponentValueAssignNotUsed,
			[Message(MessageType.Warning, "Component symbol assignment `{0}` not used. No component to assign to.")]
			ComponentSymbolAssignNotUsed,


		}

	}
}

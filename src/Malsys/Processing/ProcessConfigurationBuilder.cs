using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Malsys.Processing.Components;
using Malsys.Reflection.Components;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing {
	public class ProcessConfigurationBuilder {


		private ComponentMetadataDumper metadataDumper = new ComponentMetadataDumper();


		public ProcessConfiguration BuildConfiguration(ProcessConfigurationStatement processConfigStat, IEnumerable<ProcessComponentAssignment> componentsAssigns,
				IComponentResolver typeResolver, ProcessContext ctxt, IMessageLogger logger) {

			using (var errBlock = logger.StartErrorLoggingBlock()) {

				FSharpMap<string, ConfigurationComponent> components = MapModule.Empty<string, ConfigurationComponent>();
				var compAssignsDict = componentsAssigns.ToDictionary(ca => ca.ContainerName, ca => ca.ComponentTypeName);
				var usedCompAssigns = new System.Collections.Generic.HashSet<string>();


				// components
				foreach (var procComp in processConfigStat.Components) {
					var comp = createComponent(procComp.Name, procComp.TypeName, typeResolver, logger);
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

					var comp = createContaineredComponent(procCont.Name, procCont.TypeName, contCompTypeName, typeResolver, logger);
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

				setAndCheckUserSettableProperties(components, ctxt.Lsystem, logger);
				setAndCheckUserSettableSymbolsProperties(components, ctxt.Lsystem, logger);
				if (errBlock.ErrorOccurred) {
					return null;
				}


				var starterComponent = initializeComponents(components, ctxt, logger);

				if (errBlock.ErrorOccurred) {
					return null;
				}


				bool requiresMeasure = false;
				foreach (var kvp in components) {
					if (kvp.Value.Component is IProcessComponent) {
						requiresMeasure |= ((IProcessComponent)kvp.Value.Component).RequiresMeasure;
					}
				}

				foreach (var compAssignKvp in compAssignsDict) {
					if (!usedCompAssigns.Contains(compAssignKvp.Key)) {
						logger.LogMessage(Message.ComponentAssignNotUsed, compAssignKvp.Value, compAssignKvp.Key);
					}
				}

				return new ProcessConfiguration(components, requiresMeasure, starterComponent);
			}
		}


		private IProcessStarter initializeComponents(FSharpMap<string, ConfigurationComponent> components, ProcessContext ctxt, IMessageLogger logger) {

			IProcessStarter starterComponent = null;

			foreach (var kvp in components) {

				var comp = kvp.Value;

				try {
					comp.Component.Initialize(ctxt);
				}
				catch (ComponentInitializationException ex) {
					logger.LogMessage(Message.ComponentInitializationError, comp.Name, comp.ComponentType.FullName, ex.Message);
					continue;
				}
				catch (Exception ex) {
					logger.LogMessage(Message.ComponentInitializationException, ex.GetType().Name, comp.Name, comp.ComponentType.FullName);
					continue;
				}

				if (comp.Component is IProcessStarter) {
					if (starterComponent != null) {
						ctxt.Logger.LogMessage(Message.MoreStartComponents, typeof(IProcessStarter).Name);
					}
					starterComponent = (IProcessStarter)comp.Component;
				}
			}

			if (starterComponent == null) {
				logger.LogMessage(Message.NoStartComponent, typeof(IProcessStarter).Name);
			}

			return starterComponent;
		}


		private ConfigurationComponent createComponent(string compName, string compTypeName, IComponentResolver typeResolver, IMessageLogger logger) {

			var compType = typeResolver.ResolveComponent(compTypeName);
			if (compType == null) {
				logger.LogMessage(Message.ComponentResolveError, compName, compTypeName);
				return null;
			}

			var metadata = metadataDumper.GetMetadata(compType, logger);

			if (metadata.ComponentConstructor == null) {
				return null;
			}

			IComponent componentInstance;

			try {
				componentInstance = (IComponent)metadata.ComponentConstructor.Invoke(null);
			}
			catch (Exception ex) {
				logger.LogMessage(Message.ComponentCtorException, ex.GetType().Name, compName, compType.FullName);
				return null;
			}

			return new ConfigurationComponent(compName, componentInstance, metadata);

		}

		private ConfigurationComponent createContaineredComponent(string compName, string contTypeName, string compTypeName, IComponentResolver typeResolver, IMessageLogger logger) {

			var contType = typeResolver.ResolveComponent(contTypeName);
			if (contType == null) {
				logger.LogMessage(Message.ContainerResolveError, compName, contTypeName);
				return null;
			}

			var comp = createComponent(compName, compTypeName, typeResolver, logger);
			if (comp == null) {
				return null;
			}

			if (!contType.IsAssignableFrom(comp.ComponentType)) {
				logger.LogMessage(Message.ComponentDontFitContainer, compName, contType.FullName, comp.ComponentType.FullName);
				return null;
			}

			return comp;
		}

		/// <summary>
		///
		/// </summary>
		/// <remarks>
		/// Now it is possible to connect a connection, that is not visible (illegal) from configuration:
		/// Component in container can use properties, that are not declared in its container.
		/// </remarks>
		private void connectAndCheck(FSharpMap<string, ConfigurationComponent> components, IEnumerable<ProcessComponentsConnection> connections, IMessageLogger logger) {

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
					}

				}
			}

		}

		private void connectComponents(FSharpMap<string, ConfigurationComponent> components, ProcessComponentsConnection conn, IMessageLogger logger) {

			ConfigurationComponent srcComp;
			if (!components.TryGetValue(conn.SourceName, out srcComp)) {
				logger.LogMessage(Message.FailedToConnect, conn.SourceName, conn.TargetName, conn.TargetInputName, conn.TargetName);
				return;
			}

			ConfigurationComponent destComp;
			if (!components.TryGetValue(conn.TargetName, out destComp)) {
				logger.LogMessage(Message.FailedToConnect, conn.SourceName, conn.TargetName, conn.TargetInputName, conn.TargetInputName);
				return;
			}

			ComponentConnectablePropertyMetadata connProp;
			if (!destComp.Metadata.TryGetConnectableProperty(conn.TargetInputName, out connProp)) {
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
				if (ex.InnerException is InvalidUserValueException) {
					logger.LogMessage(Message.FailedToConnectInvalidValue, conn.SourceName, conn.TargetName, conn.TargetInputName, ex.InnerException.Message);
				}
				else {
					throw;
				}
			}
		}


		private void setAndCheckUserSettableProperties(FSharpMap<string, ConfigurationComponent> components, LsystemEvaled lsystem, IMessageLogger logger) {

			var constantsCanonicDict = lsystem.Constants.Aggregate(
				new Dictionary<string, IValue>(),
				(dict, kvp) => {
					dict[kvp.Key.ToLowerInvariant()] = kvp.Value;
					return dict;
				});

			foreach (var kvp in components) {

				var comp = kvp.Value;

				foreach (var settPropMeta in comp.Metadata.SettableProperties) {
					setSettableProperty(comp, settPropMeta, constantsCanonicDict, logger);
				}
			}

		}

		private void setAndCheckUserSettableSymbolsProperties(FSharpMap<string, ConfigurationComponent> components, LsystemEvaled lsystem, IMessageLogger logger) {

			var symbolsCanonicDict = lsystem.SymbolsConstants.Aggregate(
				new Dictionary<string, ImmutableList<Symbol<IValue>>>(),
				(dict, kvp) => {
					dict[kvp.Key.ToLowerInvariant()] = kvp.Value;
					return dict;
				});

			foreach (var kvp in components) {

				var comp = kvp.Value;

				foreach (var settSymPropMeta in comp.Metadata.SettableSymbolsProperties) {

					bool valueSet = false;

					foreach (var name in settSymPropMeta.Names) {
						ImmutableList<Symbol<IValue>> symbolsVal;
						if (symbolsCanonicDict.TryGetValue(name.ToLowerInvariant(), out symbolsVal)) {
							valueSet |= trySetPropertyValue(settSymPropMeta.PropertyInfo, comp, symbolsVal, name, null);
						}
					}

					if (settSymPropMeta.IsMandatory && !valueSet) {
						logger.LogMessage(Message.UnsetMandatoryProperty, settSymPropMeta.Names[0], comp.Name, comp.ComponentType.FullName);
					}
				}

			}
		}


		private void setSettableProperty(ConfigurationComponent comp, ComponentSettablePropertyMetadata settPropMeta, Dictionary<string, IValue> constants, IMessageLogger logger) {

			bool valueSet = false;

			foreach (var name in settPropMeta.Names) {
				IValue constVal;
				if (constants.TryGetValue(name.ToLowerInvariant(), out constVal)) {

					if (settPropMeta.PropertyType.IsAssignableFrom(constVal.GetType())) {
						valueSet |= trySetPropertyValue(settPropMeta.PropertyInfo, comp, constVal, name, null);
					}
					else {
						logger.LogMessage(Message.FailedToSetPropertyValueIncompatibleTypes,
							name, comp.Name, comp.ComponentType.FullName, constVal.Type.ToTypeString(), settPropMeta.ExpressionValueType.ToTypeString());
					}

				}
			}

			if (settPropMeta.IsMandatory && !valueSet) {
				logger.LogMessage(Message.UnsetMandatoryProperty, settPropMeta.Names[0], comp.Name, comp.ComponentType.FullName);
			}
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


		}

	}
}

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Malsys.Processing.Components;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing {
	public class ProcessConfigurationManager {


		public bool RequiresMeasure { get; private set; }
		public IProcessStarter StarterComponent { get; private set; }

		private Dictionary<string, IComponent> components = new Dictionary<string, IComponent>();



		public bool TryBuildConfiguration(ProcessConfiguration processConfig, IEnumerable<ProcessComponentAssignment> componentsAssigns,
				IComponentResolver typeResolver, ProcessContext ctxt, IMessageLogger logger) {

			ClearComponents();

			var compAssignsDict = componentsAssigns.ToDictionary(ca => ca.ContainerName, ca => ca.ComponentTypeName);

			bool errorOcured = false;

			// components
			foreach (var procComp in processConfig.Components) {
				var comp = createComponent(procComp.TypeName, typeResolver, logger);
				if (comp != null) {
					components.Add(procComp.Name, comp);
				}
				else {
					errorOcured = true;
				}
			}

			// containers
			foreach (var procCont in processConfig.Containers) {

				string contCompTypeName;
				if (!compAssignsDict.TryGetValue(procCont.Name, out contCompTypeName)) {
					contCompTypeName = procCont.DefaultTypeName;
				}

				var comp = createContaineredComponent(procCont.TypeName, contCompTypeName, typeResolver, logger);
				if (comp != null) {
					components.Add(procCont.Name, comp);
				}
				else {
					errorOcured = true;
				}
			}

			if (errorOcured) {
				// return early if some error occurred to prevent wired errors
				return false;
			}

			if (!connectAndCheck(processConfig.Connections, logger)) {
				return false;
			}

			foreach (var compKvp in components) {
				errorOcured |= !trySetAndCheckUserSettableProperties(compKvp.Value, compKvp.Key, ctxt.Lsystem, logger);
			}

			if (errorOcured) {
				return false;
			}

			initializeComponents(ctxt, logger);

			RequiresMeasure = false;
			foreach (var cp in components.Values) {
				RequiresMeasure |= cp.RequiresMeasure;
			}

			return true;
		}

		/// <summary>
		/// Cleanups all components and clears them from internal storage.
		/// </summary>
		public void ClearComponents() {

			foreach (var cp in components.Values) {
				cp.Cleanup();
			}

			components.Clear();

			StarterComponent = null;
		}

		private void initializeComponents(ProcessContext ctxt, IMessageLogger logger) {

			StarterComponent = null;

			foreach (var cp in components.Values) {
				try {
					cp.Initialize(ctxt);
				}
				catch (ComponentInitializationException ex) {
					logger.LogMessage(Message.ComponentInitializationError, cp.GetType().FullName, ex.Message);
					continue;
				}

				if (cp is IProcessStarter) {
					if (StarterComponent != null) {
						ctxt.Logger.LogMessage(Message.MoreStartComponents, typeof(IProcessStarter).Name);
					}
					StarterComponent = (IProcessStarter)cp;
				}
			}

			if (StarterComponent == null) {
				logger.LogMessage(Message.NoStartComponent, typeof(IProcessStarter).Name);
			}
		}


		private IComponent createComponent(string compTypeName, IComponentResolver typeResolver, IMessageLogger logger) {

			var compType = typeResolver.ResolveComponent(compTypeName);
			if (compType == null) {
				logger.LogMessage(Message.ComponentResolveError, compTypeName);
				return null;
			}

			if (!typeof(IComponent).IsAssignableFrom(compType)) {
				logger.LogMessage(Message.ComponentDontImplementInterface, compType.FullName, typeof(IComponent).FullName);
				return null;
			}

			var ctorInfo = compType.GetConstructor(System.Type.EmptyTypes);
			if (ctorInfo == null) {
				logger.LogMessage(Message.ComponentParamlessCtorMissing, compType.FullName);
				return null;
			}

			return (IComponent)ctorInfo.Invoke(null);
		}

		private IComponent createContaineredComponent(string contTypeName, string compTypeName, IComponentResolver typeResolver, IMessageLogger logger) {

			var contType = typeResolver.ResolveComponent(contTypeName);
			if (contType == null) {
				logger.LogMessage(Message.ContainerResolveError, contTypeName);
				return null;
			}

			var comp = createComponent(compTypeName, typeResolver, logger);
			if (comp == null) {
				return null;
			}

			if (!contType.IsAssignableFrom(comp.GetType())) {
				logger.LogMessage(Message.ComponentDontFitContainer, comp.GetType().FullName, contType.FullName);
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
		private bool connectAndCheck(IEnumerable<ProcessComponentsConnection> connections, IMessageLogger logger) {

			bool error = false;

			foreach (var compKvp in components) {

				var componentConnections = connections.Where(c => c.TargetName == compKvp.Key);

				foreach (var pi in compKvp.Value.GetType().GetProperties()) {
					var attrs = pi.GetCustomAttributes(typeof(UserConnectableAttribute), true);
					if (attrs.Length != 1) {
						continue;
					}

					var attr = (UserConnectableAttribute)attrs[0];
					var conn = componentConnections.Where(c => c.TargetInputName == pi.Name).ToList();

					if (conn.Count == 0) {
						if (!attr.IsOptional) {
							error = true;
							logger.LogMessage(Message.UnsetMandatoryConnection, compKvp.Key, pi.Name, compKvp.Value.GetType().FullName);
						}
						continue;
					}
					else if (!attr.AllowMultiple && conn.Count > 1) {
						logger.LogMessage(Message.MoreThanOneConnection, compKvp.Key, pi.Name, compKvp.Value.GetType().FullName);
					}

					error |= !tryConnect(conn[0], logger);
				}
			}

			return !error;
		}

		private bool tryConnect(ProcessComponentsConnection conn, IMessageLogger logger) {

			IComponent srcComp;
			if (!components.TryGetValue(conn.SourceName, out srcComp)) {
				logger.LogMessage(Message.FailedToConnect, conn.SourceName, conn.TargetName, conn.TargetInputName, conn.TargetName);
				return false;
			}

			IComponent destComp;
			if (!components.TryGetValue(conn.TargetName, out destComp)) {
				logger.LogMessage(Message.FailedToConnect, conn.SourceName, conn.TargetName, conn.TargetInputName, conn.TargetInputName);
				return false;
			}

			var prop = destComp.GetType().GetProperty(conn.TargetInputName);
			if (prop == null) {
				logger.LogMessage(Message.FailedToConnectPropertyDontExist, conn.SourceName, conn.TargetName, conn.TargetInputName);
				return false;
			}

			if (!prop.PropertyType.IsAssignableFrom(srcComp.GetType())) {
				logger.LogMessage(Message.FailedToConnectNotAssignable, conn.SourceName, conn.TargetName, conn.TargetInputName);
				return false;
			}

			prop.SetValue(destComp, srcComp, null);
			return true;
		}


		private bool trySetAndCheckUserSettableProperties(IComponent component, string componentConfigName, LsystemEvaled lsystem, IMessageLogger logger) {

			bool error = false;

			var symbolsCanonicDict = lsystem.SymbolsConstants.ToDictionary(x => x.Key.ToLower(), x => x.Value);
			var constantsCanonicDict = lsystem.Constants.ToDictionary(x => x.Key.ToLower(), x => x.Value);

			foreach (var propInfo in component.GetType().GetProperties()) {

				var attr = propInfo.GetCustomAttributes(typeof(UserSettableAttribute), true);
				if (attr.Length != 1) {
					continue;
				}

				bool mandatory = ((UserSettableAttribute)attr[0]).IsMandatory;
				bool valueSet = false;
				string nameLower = propInfo.Name.ToLowerInvariant();

				ImmutableList<Symbol<IValue>> symbolsVal;
				if (symbolsCanonicDict.TryGetValue(nameLower, out symbolsVal)) {
					if (propInfo.PropertyType.Equals(typeof(ImmutableList<Symbol<IValue>>))) {
						valueSet |= trySetPropertyValue(propInfo, component, symbolsVal, component, componentConfigName, logger);
					}
					else {
						logger.LogMessage(Message.ExpectedSymbolListAsValue, propInfo.Name, component.GetType().FullName);
					}
				}


				IValue constVal;
				if (constantsCanonicDict.TryGetValue(nameLower, out constVal)) {

					if (propInfo.PropertyType.Equals(typeof(IValue))) {
						valueSet |= trySetPropertyValue(propInfo, component, constVal, component, componentConfigName, logger);
					}
					else if (propInfo.PropertyType.Equals(typeof(Constant))) {
						if (constVal.IsConstant) {
							valueSet |= trySetPropertyValue(propInfo, component, constVal, component, componentConfigName, logger);
						}
						else {
							logger.LogMessage(Message.ExpectedConstantAsValue, propInfo.Name, componentConfigName, component.GetType().FullName);
						}
					}
					else if (propInfo.PropertyType.Equals(typeof(ValuesArray))) {
						if (constVal.IsArray) {
							valueSet |= trySetPropertyValue(propInfo, component, constVal, component, componentConfigName, logger);
						}
						else {
							logger.LogMessage(Message.ExpectedArrayAsValue, propInfo.Name, componentConfigName, component.GetType().FullName);
						}
					}
					else {
						logger.LogMessage(Message.ExpectedIValueAsValue, propInfo.Name, componentConfigName, component.GetType().FullName);
					}
				}

				if (mandatory && !valueSet) {
					logger.LogMessage(Message.UnsetMandatoryProperty, propInfo.Name, componentConfigName, component.GetType().FullName);
					error = true;
				}

			}

			return !error;
		}

		bool trySetPropertyValue(PropertyInfo pi, object obj, object value, IComponent component, string configName, IMessageLogger logger) {

			try {
				pi.SetValue(obj, value, null);
				return true;
			}
			catch (TargetInvocationException ex) {
				if (ex.InnerException is InvalidUserValueException) {
					logger.LogMessage(Message.FailedToSetPropertyValue, pi.Name, configName, component.GetType().FullName, ex.InnerException.Message);
				}
				else {
					logger.LogMessage(Message.SetPropertyValueError, pi.Name, configName, component.GetType().FullName,
						(ex.InnerException ?? ex).GetType().Name);
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
			[Message(MessageType.Error, "Failed to resolve component with type `{0}`.")]
			ComponentResolveError,
			[Message(MessageType.Error, "Failed to resolve container with type `{0}`.")]
			ContainerResolveError,
			[Message(MessageType.Error, "Component `{0}` does not implement required interface `{1}`.")]
			ComponentDontImplementInterface,
			[Message(MessageType.Error, "Component `{0}` does not have parameter-less constructor.")]
			ComponentParamlessCtorMissing,
			[Message(MessageType.Error, "Component `{0}` in not compatible with container `{1}`.")]
			ComponentDontFitContainer,
			[Message(MessageType.Error, "Failed to connect `{0}` and `{1}.{2}`, `{3}` not found.")]
			FailedToConnect,
			[Message(MessageType.Error, "Failed to connect `{0}` and `{1}.{2}`, public property `{2}` on `{1}` does not exist.")]
			FailedToConnectPropertyDontExist,
			[Message(MessageType.Error, "Failed to connect `{0}` and `{1}.{2}`, `{0}` is not assignable to `{1}.{2}`.")]
			FailedToConnectNotAssignable,
			[Message(MessageType.Error, "Exception `{3}` was thrown on set value of property `{0}` of `{1}` (`{2}`).")]
			SetPropertyValueError,
			[Message(MessageType.Error, "Failed initialize component `{0}`. {1}")]
			ComponentInitializationError,


			[Message(MessageType.Warning, "Expected constant as value of property `{0}` of `{1}` (`{2}`).")]
			ExpectedConstantAsValue,
			[Message(MessageType.Warning, "Expected array as value of property `{0}` of `{1}` (`{2}`).")]
			ExpectedArrayAsValue,
			[Message(MessageType.Warning, "Expected symbols list as value of property `{0}` of `{1}` (`{2}`).")]
			ExpectedSymbolListAsValue,
			[Message(MessageType.Warning, "Expected constant or array as value of property `{0}` of `{1}` (`{2}`).")]
			ExpectedIValueAsValue,
			[Message(MessageType.Warning, "Failed to set value of property `{0}` of `{1}` (`{2}`). {3}")]
			FailedToSetPropertyValue,


		}

	}
}

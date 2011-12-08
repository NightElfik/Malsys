using System;
using System.Collections.Generic;
using System.Linq;
using Malsys.Processing.Components;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Core;

namespace Malsys.Processing {
	public class ProcessConfigurationManager {


		public bool RequiresMeasure { get; private set; }
		public IProcessStarter StarterComponent { get; private set; }

		private Dictionary<string, IComponent> components = new Dictionary<string, IComponent>();



		public void BuildConfiguration(ProcessConfiguration processConfig, IEnumerable<ProcessComponentAssignment> componentsAssigns,
				IComponentResolver typeResolver, ProcessContext ctxt) {

			ClearComponents();

			var compAssignsDict = componentsAssigns.ToDictionary(ca => ca.ContainerName, ca => ca.ComponentTypeName);

			// components
			foreach (var procComp in processConfig.Components) {
				var comp = createComponent(procComp.TypeName, typeResolver);
				components.Add(procComp.Name, comp);
			}

			// containers
			foreach (var procCont in processConfig.Containers) {

				string contCompTypeName;
				if (!compAssignsDict.TryGetValue(procCont.Name, out contCompTypeName)) {
					contCompTypeName = procCont.DefaultTypeName;
				}

				var comp = createComponent(procCont.TypeName, contCompTypeName, typeResolver);
				components.Add(procCont.Name, comp);
			}

			// connections
			// it is possible to connect a connection, that is not visible (illegal) from config:
			// component in container can use properties, that are not declared in ints container
			foreach (var conn in processConfig.Connections) {
				connect(conn.SourceName, conn.TargetName, conn.TargetInputName);
			}


			initializeComponents(ctxt);

		}


		public void ClearComponents() {

			foreach (var cp in components.Values) {
				cp.Cleanup();
			}

			components.Clear();

			StarterComponent = null;
		}


		private void initializeComponents(ProcessContext ctxt) {

			foreach (var cp in components.Values) {
				cp.Initialize(ctxt);
				setUserSettableProperties(cp, ctxt);

				if (cp is IProcessStarter) {
					if (StarterComponent != null) {
						throw new ApplicationException("Configuration can not have more than one `{0}` component.".Fmt(typeof(IProcessStarter)));
					}
					StarterComponent = (IProcessStarter)cp;
				}
			}

			RequiresMeasure = false;
			foreach (var cp in components.Values) {
				RequiresMeasure |= cp.RequiresMeasure;
			}
		}


		private IComponent createComponent(string compTypeName, IComponentResolver typeResolver) {

			var compType = typeResolver.ResolveComponent(compTypeName);
			if (compType == null) {
				throw new ApplicationException("Failed to resolve component with type `{0}`.".Fmt(compTypeName));
			}

			if (!typeof(IComponent).IsAssignableFrom(compType)) {
				throw new ApplicationException("Component `{0}` does not implemet `{1}` interface."
					.Fmt(compType.FullName, typeof(IComponent).FullName));
			}

			var ctorInfo = compType.GetConstructor(System.Type.EmptyTypes);
			if(ctorInfo == null){
				throw new ApplicationException("Component `{0}` does not have prameter-less constructor."
					.Fmt(compType.FullName));
			}

			return (IComponent)ctorInfo.Invoke(null);
		}

		private IComponent createComponent(string contTypeName, string compTypeName, IComponentResolver typeResolver) {

			var contType = typeResolver.ResolveContainer(contTypeName);
			if (contType == null) {
				throw new ApplicationException("Failed to resolve cotainer with type `{0}`.".Fmt(contTypeName));
			}

			var comp = createComponent(compTypeName, typeResolver);

			if (!contType.IsAssignableFrom(comp.GetType())) {
				throw new ApplicationException("Component `{0}` in not compatible with container `{1}`."
					.Fmt(comp.GetType().FullName, contType.FullName));
			}

			return comp;
		}

		private void connect(string srcCompName, string destCompName, string destMemberName) {

			IComponent srcComp;
			if (!components.TryGetValue(srcCompName, out srcComp)) {
				throw new ApplicationException("Failed to connect `{0}` and `{1}.{2}`, `{0}` not found."
					.Fmt(srcCompName, destCompName, destMemberName));
			}

			IComponent destComp;
			if (!components.TryGetValue(destCompName, out destComp)) {
				throw new ApplicationException("Failed to connect `{0}` and `{1}.{2}`, `{1}` not found."
					.Fmt(srcCompName, destCompName, destMemberName));
			}

			var prop = destComp.GetType().GetProperty(destMemberName);
			if (prop == null) {
				throw new ApplicationException("Failed to connect `{0}` and `{1}.{2}`, public property `{2}` on `{1}` does not exist."
					.Fmt(srcCompName, destCompName, destMemberName));
			}

			if (!prop.PropertyType.IsAssignableFrom(srcComp.GetType())) {
				throw new ApplicationException("Failed to connect `{0}` and `{1}.{2}`, `{0}` is not assignable to `{1}.{2}`."
					.Fmt(srcCompName, destCompName, destMemberName));
			}

			prop.SetValue(destComp, srcComp, null);

		}


		private void setUserSettableProperties(IComponent component, ProcessContext ctxt) {

			foreach (var propInfo in component.GetType().GetProperties()) {

				if (propInfo.GetCustomAttributes(typeof(UserSettableAttribute), true).Length != 1) {
					continue;
				}

				if (propInfo.PropertyType.Equals(typeof(IValue))) {
					var maybeConst = ctxt.Lsystem.Constants.TryFind(propInfo.Name.ToLowerInvariant());
					if (OptionModule.IsSome(maybeConst)) {
						propInfo.SetValue(component, maybeConst.Value, null);
					}
				}
				if (propInfo.PropertyType.Equals(typeof(Constant))) {
					var maybeConst = ctxt.Lsystem.Constants.TryFind(propInfo.Name.ToLowerInvariant());
					if (OptionModule.IsSome(maybeConst) && maybeConst.Value.IsConstant) {
						propInfo.SetValue(component, (Constant)maybeConst.Value, null);
					}
				}
				if (propInfo.PropertyType.Equals(typeof(ValuesArray))) {
					var maybeConst = ctxt.Lsystem.Constants.TryFind(propInfo.Name.ToLowerInvariant());
					if (OptionModule.IsSome(maybeConst) && maybeConst.Value.IsArray) {
						propInfo.SetValue(component, (ValuesArray)maybeConst.Value, null);
					}
				}
				else if (propInfo.PropertyType.Equals(typeof(ImmutableList<Symbol<IValue>>))) {
					var maybeSyms = ctxt.Lsystem.SymbolsConstants.TryFind(propInfo.Name.ToLowerInvariant());
					if (OptionModule.IsSome(maybeSyms)) {
						propInfo.SetValue(component, maybeSyms.Value, null);
					}
				}

			}

		}

	}
}

using System;
using System.Collections.Generic;

namespace Malsys.Processing {
	public class ComponentResolver : IComponentContainer, IComponentResolver {

		private Dictionary<string, Type> components = new Dictionary<string, Type>();


		public void RegisterComponent(string name, Type type, bool ignoreConflicts = true) {

			if (components.ContainsKey(name)) {
				if (ignoreConflicts) {
					components[name] = type;
				}
				else {
					throw new ArgumentException("Component `{0}` already registered to type `{1}`.".Fmt(name, components[name]));
				}
			}
			else {
				components.Add(name, type);
			}

		}


		public Type ResolveComponent(string name) {

			Type result;
			if (components.TryGetValue(name, out result)) {
				return result;
			}
			else {
				return null;
			}
		}

		public IEnumerable<KeyValuePair<string, Type>> GetAllRegisteredComponents() {
			return components;
		}

	}

	public static class IComponentResolverExtensions {

		public static void RegisterComponent(this IComponentContainer container, Type t, bool ignoreConflicts = true) {
			container.RegisterComponent(t.Name, t, ignoreConflicts);
		}

		public static void RegisterComponentNameAndFullName(this IComponentContainer container, Type t, bool ignoreConflicts = true) {
			container.RegisterComponent(t.Name, t, ignoreConflicts);
			container.RegisterComponent(t.FullName, t, ignoreConflicts);
		}

	}
}

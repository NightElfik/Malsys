using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Malsys.Processing {
	public class ComponentResolver : IComponentContainer, IComponentResolver {

		private Dictionary<string, Type> components = new Dictionary<string, Type>();
		private Dictionary<string, Type> containers = new Dictionary<string, Type>();


		public void RegisterComponent(string name, Type type, bool replaceIfExists = false) {

			if (components.ContainsKey(name)) {
				if (replaceIfExists) {
					components[name] = type;
				}
				else {
					throw new ArgumentException("Component `{0}` alredy registered to type `{1}`".Fmt(name, components[name]));
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

	}

	public static class IComponentResolverExtensions {

		public static void RegisterComponent(this IComponentContainer container, Type t, bool replaceIfExists = false) {
			container.RegisterComponent(t.Name, t, replaceIfExists);
		}

		public static void RegisterComponentNameAndFullName(this IComponentContainer container, Type t, bool replaceIfExists = false) {
			container.RegisterComponent(t.Name, t, replaceIfExists);
			container.RegisterComponent(t.FullName, t, replaceIfExists);
		}

	}
}

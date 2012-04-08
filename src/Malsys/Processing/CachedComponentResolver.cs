using System;
using System.Collections.Generic;
using Malsys.Reflection.Components;

namespace Malsys.Processing {
	public class CachedComponentResolver : IComponentMetadataResolver, IComponentMetadataContainer {

		private Dictionary<string, Type> components = new Dictionary<string, Type>();

		private ComponentMetadataDumper metadataDumper = new ComponentMetadataDumper();

		private bool isCacheComplete = false;
		private Dictionary<string, ComponentMetadata> metadataCache = new Dictionary<string, ComponentMetadata>();


		public void RegisterComponentType(string name, Type type, bool ignoreConflicts = true) {

			if (components.ContainsKey(name)) {
				if (ignoreConflicts) {
					components[name] = type;
					metadataCache.Remove(name);  // remove possible cached metadata
					isCacheComplete = false;
				}
				else {
					throw new ArgumentException("Component `{0}` already registered to type `{1}`.".Fmt(name, components[name]));
				}
			}
			else {
				components.Add(name, type);
				isCacheComplete = false;
			}

		}


		public Type ResolveComponentType(string name) {

			Type componentType;
			if (components.TryGetValue(name, out componentType)) {
				return componentType;
			}
			else {
				return null;
			}
		}

		public IEnumerable<KeyValuePair<string, Type>> GetAllRegisteredComponentTypes() {
			return components;
		}


		/// <summary>
		/// Thread safe.
		/// </summary>
		/// <remarks>
		/// Access to the cache is not synchronized but in the worst case
		/// metadata will be dumped more than once.
		/// </remarks>
		public ComponentMetadata ResolveComponentMetadata(string name, IMessageLogger logger) {

			ComponentMetadata componentMetadata;
			if (metadataCache.TryGetValue(name, out componentMetadata)) {
				return componentMetadata;
			}

			Type componentType;
			if (!components.TryGetValue(name, out componentType)) {
				return null;
			}

			using (var errBlock = logger.StartErrorLoggingBlock()) {
				componentMetadata = metadataDumper.GetMetadata(componentType, logger);
				if (errBlock.ErrorOccurred) {
					return null;  // do not cache or return metadata when error occurred
				}
				metadataCache.Add(name, componentMetadata);
				return componentMetadata;
			}
		}

		public IEnumerable<KeyValuePair<string, ComponentMetadata>> GetAllRegisteredComponentsMetadata(IMessageLogger logger) {

			// ensure that all components metadata are in cache
			if (!isCacheComplete) {
				foreach (string name in components.Keys) {
					ResolveComponentMetadata(name, logger);
				}
				isCacheComplete = true;
			}

			return metadataCache;
		}




	}

	public static class IComponentResolverExtensions {

		public static void RegisterComponent(this IComponentTypeContainer container, Type t, bool ignoreConflicts = true) {
			container.RegisterComponentType(t.Name, t, ignoreConflicts);
		}

		public static void RegisterComponentNameAndFullName(this IComponentTypeContainer container, Type t, bool ignoreConflicts = true) {
			container.RegisterComponentType(t.Name, t, ignoreConflicts);
			container.RegisterComponentType(t.FullName, t, ignoreConflicts);
		}

	}
}

using System.Collections.Generic;
using Malsys.Reflection.Components;

namespace Malsys.Processing {
	public class ComponentResolver : IComponentMetadataResolver, IComponentMetadataContainer {

		private Dictionary<string, ComponentMetadata> components = new Dictionary<string, ComponentMetadata>();


		public ComponentMetadata ResolveComponentMetadata(string name) {

			ComponentMetadata componentMetadata;
			if (components.TryGetValue(name, out componentMetadata)) {
				return componentMetadata;
			}

			return null;
		}

		public void RegisterComponentMetadata(string name, ComponentMetadata metadata) {
			components[name] = metadata;
		}

		public IEnumerable<KeyValuePair<string, ComponentMetadata>> GetAllRegisteredComponents() {
			return components;
		}

	}
}

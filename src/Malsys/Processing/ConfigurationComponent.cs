using System;
using Malsys.Processing.Components;
using Malsys.Reflection.Components;

namespace Malsys.Processing {
	/// <summary>
	/// Component in process configuration.
	/// </summary>
	public class ConfigurationComponent {

		public readonly string Name;

		public readonly IComponent Component;

		public readonly Type ComponentType;

		public readonly ComponentMetadata Metadata;


		public ConfigurationComponent(string name, IComponent component, ComponentMetadata metadata) {

			Name = name;
			Component = component;
			ComponentType = metadata.ComponentType;
			Metadata = metadata;

		}

	}
}

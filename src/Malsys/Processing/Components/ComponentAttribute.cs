using System;

namespace Malsys.Processing.Components {
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
	public sealed class ComponentAttribute : Attribute {

		public string Name { get; private set; }
		public string Group { get; private set; }

		public ComponentAttribute(string name, string group) {
			Name = name;
			Group = group;
		}

	}
}

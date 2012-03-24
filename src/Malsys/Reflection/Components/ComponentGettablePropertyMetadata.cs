﻿using System.Reflection;

namespace Malsys.Reflection.Components {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ComponentGettablePropertyMetadata {

		public readonly ImmutableList<string> Names;

		public readonly bool GettableBeforeInitialiation;

		public readonly PropertyInfo PropertyInfo;


		public ComponentGettablePropertyMetadata(ImmutableList<string> names, bool gettableBeforeInitialiation, PropertyInfo propertyInfo) {

			Names = names;
			GettableBeforeInitialiation = gettableBeforeInitialiation;
			PropertyInfo = propertyInfo;

		}

	}
}
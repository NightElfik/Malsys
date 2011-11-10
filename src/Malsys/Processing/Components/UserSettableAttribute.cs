using System;

namespace Malsys.Processing.Components {
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class UserSettableAttribute : Attribute {

		public UserSettableAttribute() {

		}

	}
}

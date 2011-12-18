using System;

namespace Malsys.Processing.Components {
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class UserConnectableAttribute : Attribute {

		public UserConnectableAttribute() {

		}

	}
}

using System;

namespace Malsys.Processing.Components {
	/// <remarks>
	/// Attribute inherance do not work on properties in interface.
	/// So do not forget to add it to derived properties too.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class UserSettableAttribute : Attribute {

		public UserSettableAttribute() {

		}

	}
}

using System;

namespace Malsys.Processing.Components {
	/// <remarks>
	/// Attribute inherence do not work on properties in interface,
	/// so do not forget to add it to derived properties too.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class UserSettableAttribute : Attribute {

		public UserSettableAttribute() {

		}

	}
}

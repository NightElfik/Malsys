using System;

namespace Malsys.Processing.Components {
	/// <summary>
	/// Attribute for marking properties on IComponent connectable with user defined configuration.
	/// </summary>
	/// <remarks>
	/// Attribute inherence do not work on properties in interface,
	/// do not forget to add it to derived types too.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class UserConnectableAttribute : Attribute {

		/// <summary>
		/// Indicating whether component connection is optional.
		/// Default false.
		/// </summary>
		public bool IsOptional { get; set; }

		/// <summary>
		/// Indicating whether more than one component can be connected.
		/// Default false.
		/// </summary>
		public bool AllowMultiple { get; set; }


	}
}

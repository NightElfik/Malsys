using System;

namespace Malsys.Processing.Components {
	/// <summary>
	/// Indicates that marked property value can be read by user from input code.
	/// </summary>
	/// <remarks>
	/// Marked property must have public getter.
	/// Property type must be assignable to IValue.
	/// Attribute inherence do not work on properties in interface, do not forget to add it to derived types too.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class UserGettableAttribute : Attribute {

	}

}

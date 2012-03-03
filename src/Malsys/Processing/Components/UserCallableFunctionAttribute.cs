using System;

namespace Malsys.Processing.Components {
	/// <summary>
	/// Indicates that marked method can be called by user from input code.
	/// </summary>
	/// <remarks>
	/// Marked method must have just one parameter of type ArgsStorage.
	/// Return type must be assignable to IValue.
	/// Attribute inherence do not work on properties in interface, do not forget to add it to derived types too.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public sealed class UserCallableFunctionAttribute : Attribute {

	}
}

using System;

namespace Malsys.Resources {
	/// <summary>
	/// Indicates that marked class can contain Malsys function definitions.
	/// </summary>
	/// <remarks>
	/// Function definition fields must be static and type of FunctionCore.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class MalsysFunctionsAttribute : Attribute {

	}
}

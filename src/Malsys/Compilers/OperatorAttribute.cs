using System;

namespace Malsys.Compilers {
	/// <summary>
	/// Indicates that marked field is operator.
	/// </summary>
	/// <remarks>
	/// Marked field must be static and type of OperatorCore.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class OperatorAttribute : Attribute {

	}
}

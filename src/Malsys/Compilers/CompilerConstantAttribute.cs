using System;

namespace Malsys.Compilers {
	/// <summary>
	/// Indicates that marked field is compiler constant.
	/// </summary>
	/// <remarks>
	/// Marked field must be type of double.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class CompilerConstantAttribute : Attribute {

	}
}

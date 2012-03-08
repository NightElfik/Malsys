using System;

namespace Malsys.Compilers {
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class CompilerConstantAttribute : Attribute {

	}
}

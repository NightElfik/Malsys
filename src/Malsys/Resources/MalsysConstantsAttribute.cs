﻿using System;

namespace Malsys.Resources {
	/// <summary>
	/// Indicates that marked class can contain compiler compiler constants.
	/// </summary>
	/// <remarks>
	/// Compiler constants fields must be static and type of double.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class MalsysConstantsAttribute : Attribute {

	}
}

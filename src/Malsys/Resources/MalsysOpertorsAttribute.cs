/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;

namespace Malsys.Resources {
	/// <summary>
	/// Indicates that marked class can contain Malsys operators definitions.
	/// </summary>
	/// <remarks>
	/// Operator definition fields must be static and type of OperatorCore.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class MalsysOpertorsAttribute : Attribute {

	}
}

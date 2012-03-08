﻿using System;

namespace Malsys.Processing.Components {
	/// <summary>
	/// Indicates that marked property value can be read by user in input code.
	/// Value can be used in L-system local variables only if GettableBeforeInitialiation is set to true.
	/// In this case the read operation will occur on non-initialized component.
	/// Also when component is Settable and GettableBeforeInitialiation is true its value will be set by itself.
	/// </summary>
	/// <remarks>
	/// Marked property must have public getter.
	/// Property type must be assignable to IValue.
	/// Attribute inherence do not work on properties in interface, do not forget to add it to derived types too.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class UserGettableAttribute : Attribute {

		public bool GettableBeforeInitialiation { get; set; }

	}

}

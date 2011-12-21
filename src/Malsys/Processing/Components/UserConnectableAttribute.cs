using System;

namespace Malsys.Processing.Components {
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

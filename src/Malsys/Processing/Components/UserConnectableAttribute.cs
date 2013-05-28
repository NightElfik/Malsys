// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Runtime.Serialization;

namespace Malsys.Processing.Components {
	/// <summary>
	/// Indicates that another component can be connected to marked property.
	/// </summary>
	/// <remarks>
	/// Marked property must have public setter.
	/// Property type must be assignable to IComponent.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
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

	/// <summary>
	/// Thrown by properties marked with UserConnectableAttribute when value is invalid.
	/// </summary>
	[Serializable]
	public class InvalidConnectedComponentException : ComponentException {

		public InvalidConnectedComponentException() { }
		public InvalidConnectedComponentException(string message) : base(message) { }
		public InvalidConnectedComponentException(string message, Exception inner) : base(message, inner) { }

		protected InvalidConnectedComponentException(SerializationInfo info, StreamingContext context) : base(info, context) { }

	}

}

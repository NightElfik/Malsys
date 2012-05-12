/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Runtime.Serialization;

namespace Malsys.Processing.Components {
	/// <summary>
	/// Indicates that marked property value can be set by user from input code.
	/// </summary>
	/// <remarks>
	/// Marked property must have public setter (public getter is not required).
	/// Property type must be assignable to IValue.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class UserSettableAttribute : Attribute {

		/// <summary>
		/// Indicating whether marked property has to be set by user.
		/// Default false.
		/// </summary>
		public bool IsMandatory { get; set; }



	}

	/// <summary>
	/// Indicates that marked property value can be set by user from input code.
	/// </summary>
	/// <remarks>
	/// Marked property must have public setter.
	/// Property type must be type of ImmutableList{Symbol{IValue}}.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class UserSettableSybolsAttribute : Attribute {

		/// <summary>
		/// Indicating whether marked property has to be set by user.
		/// Default false.
		/// </summary>
		public bool IsMandatory { get; set; }


	}

	/// <summary>
	/// Thrown by properties marked with UserSettableAttribute when value is invalid.
	/// </summary>
	[Serializable]
	public class InvalidUserValueException : ComponentException {

		public InvalidUserValueException() { }
		public InvalidUserValueException(string message) : base(message) { }
		public InvalidUserValueException(string message, Exception inner) : base(message, inner) { }

		protected InvalidUserValueException(SerializationInfo info, StreamingContext context) : base(info, context) { }

	}

}

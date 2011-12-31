using System;

namespace Malsys.Processing.Components {
	/// <summary>
	/// Attribute for marking properties on IComponent to allow user to set them from input code.
	/// </summary>
	/// <remarks>
	/// Attribute inherence do not work on properties in interface,
	/// do not forget to add it to derived types too.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class UserSettableAttribute : Attribute {

		/// <summary>
		/// Indicating whether marked property has to be set by user.
		/// Default false.
		/// </summary>
		public bool IsMandatory { get; set; }


	}

	/// <summary>
	/// Thrown by properties with UserSettableAttribute.
	/// </summary>
	[Serializable]
	public class InvalidUserValueException : ApplicationException {

		public InvalidUserValueException(string message)
			: base(message) {
		}

	}

}

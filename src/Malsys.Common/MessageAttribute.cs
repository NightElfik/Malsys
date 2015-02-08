using System;

namespace Malsys {
	/// <summary>
	/// This attribute is intended for marking values of enumeration for logging messages.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class MessageAttribute : Attribute {

		/// <summary>
		/// Message can contain arguments slots like "{0}" (zero in braces).
		/// </summary>
		public string Message { get; private set; }

		public MessageType MessageType { get; private set; }


		public MessageAttribute(MessageType messageType, string message) {
			MessageType = messageType;
			Message = message;
		}

	}
}

using System;

namespace Malsys {
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class MessageAttribute : Attribute {

		public string Message { get; private set; }

		public MessageType MessageType { get; private set; }


		public MessageAttribute(MessageType messageType, string message) {
			MessageType = messageType;
			Message = message;
		}

	}
}

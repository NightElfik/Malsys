using System;

namespace Malsys {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Message {

		public readonly string Id;
		public readonly string MessageStr;
		public readonly MessageType Type;
		public readonly string SourceName;
		public readonly Position Position;
		public readonly DateTime Time;


		public Message(string id, MessageType type, string message, string source, Position position) {

			Id = id;
			MessageStr = message;
			Type = type;
			SourceName = source;
			Position = position;
			Time = DateTime.Now;
		}

		public string GetFullMessage() {
			return "{0} [{1}]: {2} In `{3}` {4}.".Fmt(Type.ToString(), Id, MessageStr, SourceName, Position);
		}

		public override string ToString() {
			return MessageStr;
		}
	}

	public enum MessageType {
		Error, Warning, Info
	}
}

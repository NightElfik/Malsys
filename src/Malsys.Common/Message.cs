/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;

namespace Malsys {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class Message {

		public readonly string Id;
		public readonly string MessageStr;
		public readonly MessageType Type;
		public readonly Position Position;
		public readonly DateTime Time;


		public Message(string id, MessageType type, string message, Position position, DateTime time) {

			Id = id;
			MessageStr = message;
			Type = type;
			Position = position;
			Time = time;
		}

		public string GetFullMessage() {
			return "{0} [{1}] at {2}: {3} ".Fmt(Type.ToString(), Id, Position, MessageStr);
		}

		public override string ToString() {
			return MessageStr;
		}
	}

	public enum MessageType {
		Error,
		Warning,
		Info
	}
}

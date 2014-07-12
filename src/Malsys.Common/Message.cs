// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;

namespace Malsys {
	public class Message {

		public string Id;
		public string MessageStr;
		public MessageType Type;
		public PositionRange Position;
		public DateTime Time;


		public Message(string id, MessageType type, string message, PositionRange position, DateTime time) {
			Id = id;
			MessageStr = message;
			Type = type;
			Position = position;
			Time = time;
		}

		public string GetFullMessage() {
			if (Position.IsUnknown) {
				return "{0} [{1}]: {2} ".Fmt(Type.ToString(), Id, MessageStr);
			}
			else {
				return "{0} [{1}] at {2}: {3} ".Fmt(Type.ToString(), Id, Position, MessageStr);
			}
		}

		public override string ToString() {
			return MessageStr;
		}
	}

	public enum MessageType {
		Error,
		Warning,
		Info,
		Statistics,
	}
}

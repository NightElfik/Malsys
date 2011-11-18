using System;

namespace Malsys {
	public class Message {

		public int MessageId { get; private set; }
		public string MessageStr { get; private set; }
		public MessageType Type { get; private set; }
		public string SourceName { get; private set; }
		public Position Position { get; private set; }
		public ImmutableList<Position> OtherPositions { get; private set; }


		public Message(string message, MessageType type, string source, Position position) {
			MessageStr = message;
			Type = type;
			SourceName = source;
			Position = position;
			OtherPositions = ImmutableList<Position>.Empty;
		}

		public Message(string message, MessageType type, string source, Position position, params Position[] otherPositions) {
			MessageStr = message;
			Type = type;
			SourceName = source;
			Position = position;
			OtherPositions = new ImmutableList<Position>(otherPositions);
		}

		public string GetFullMessage() {
			return "{0}: {1} In `{2}` from line {3} col {4} to line {5} col {6}.".Fmt(
				Type.ToString(), MessageStr, SourceName, Position.BeginLine, Position.BeginColumn, Position.EndLine, Position.EndColumn);
		}

		public override string ToString() {
			return MessageStr;
		}
	}

	public enum MessageType {
		Error, Warning, Info
	}
}

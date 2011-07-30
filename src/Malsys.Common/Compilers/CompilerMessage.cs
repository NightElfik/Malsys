using System;

namespace Malsys.Compilers {
	public class CompilerMessage {

		public string Message { get; private set; }
		public CompilerMessageType Type { get; private set; }
		public string SourceName { get; private set; }
		public Position Position { get; private set; }


		public CompilerMessage(string message, CompilerMessageType type, string source, Position position) {
			Message = message;
			Type = type;
			SourceName = source;
			Position = position;
		}

		public string GetFullMessage() {
			return "{0}: {1} In {2} from line {3} col {4} to line {5} col {6}".Fmt(
				Type.ToString(), Message, SourceName, Position.BeginLine, Position.BeginColumn, Position.EndLine, Position.EndColumn);
		}
	}

	public enum CompilerMessageType {
		Error, Warning, Notice
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Ast;

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
	}

	public enum CompilerMessageType {
		Error, Warning, Notice
	}
}

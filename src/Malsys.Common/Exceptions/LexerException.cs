using System;
using System.Runtime.Serialization;
using Microsoft.FSharp.Text.Lexing;

namespace Malsys {
	/// <summary>
	/// Lexer exception for F# lexer. It is really hell to write this class in F#.
	/// </summary>
	[Serializable]
	public class LexerException : Exception {
		public string LexerMessage { get; private set; }
		public Position Position { get; private set; }

		public LexerException() { }
		public LexerException(string message, Position position) {
			LexerMessage = message;
			Position = position;
		}
		public LexerException(string message, Position position, Exception inner)
			: base(message, inner) {

			LexerMessage = message;
			Position = position;
		}

		public override string Message {
			get {
				return "{0} in `{1}` at line {2} col {3}.".Fmt(LexerMessage.TrimEnd('.'), Position.FileName, Position.Line, Position.Column);
			}
		}

		protected LexerException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}
}

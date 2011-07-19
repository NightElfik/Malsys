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
		public Position BeginPosition { get; private set; }
		public Position EndPosition { get; private set; }

		public LexerException() { }
		public LexerException(string message, Position beginPosition, Position endPosition) {
			LexerMessage = message;
			BeginPosition = beginPosition;
			EndPosition = endPosition;
		}
		public LexerException(string message, Position beginPosition, Position endPosition, Exception inner)
			: base(message, inner) {

			LexerMessage = message;
			BeginPosition = beginPosition;
			EndPosition = endPosition;
		}

		public override string Message {
			get {
				return "{0} in `{1}` at line {2} col {3}.".Fmt(LexerMessage.TrimEnd('.'), BeginPosition.FileName, BeginPosition.Line, BeginPosition.Column);
			}
		}

		protected LexerException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}
}

using System;
using System.Runtime.Serialization;
using Microsoft.FSharp.Text.Parsing;

namespace Malsys {
	/// <summary>
	/// Parser exception for F# parser. It is really hell to write this class in F#.
	/// </summary>
	[Serializable]
	public class ParserException<Tok> : Exception {
		public string ParserMessage { get; private set; }
		public ParseErrorContext<Tok> ErrorContext { get; private set; }

		public ParserException() { }
		public ParserException(ParseErrorContext<Tok> ctxt) {
			ErrorContext = ctxt;
		}
		public ParserException(string message, ParseErrorContext<Tok> ctxt) {
			ParserMessage = message;
			ErrorContext = ctxt;
		}
		public ParserException(string message, ParseErrorContext<Tok> ctxt, Exception inner)
			: base(message, inner) {

			ParserMessage = message;
			ErrorContext = ctxt;
		}

		public override string Message {
			get {
				var pos = ErrorContext.ParseState.ResultRange.Item1;
				return "Parser error `{0}` in `{1}` at line {2} col {3}.".Fmt(ErrorContext.Message.TrimEnd('.'), pos.FileName, pos.Line, pos.Column);
			}
		}

		protected ParserException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}
}

/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Parsing {
	/// <summary>
	/// Message enumeration for the lexer from the Malsys.Parser (I don't know how to write this in the F#).
	/// </summary>
	public enum LexerMessage {

		[Message(MessageType.Error, "Unrecognized input `{0}`.")]
		UnrecognizedInput,
		[Message(MessageType.Error, "Failed to parse `{0}` as integer.")]
		FailedParseInt,
		[Message(MessageType.Error, "Failed to parse `{0}` as floating point number.")]
		FailedParseFloat,

		[Message(MessageType.Warning, "Unterminated multiline comment.")]
		UnterminatedMultilineComment,

	}
}

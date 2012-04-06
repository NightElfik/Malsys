
namespace Malsys.Parsing {
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

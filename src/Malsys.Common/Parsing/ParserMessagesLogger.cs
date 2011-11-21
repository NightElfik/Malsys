using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.Parsing {
	public class ParserMessagesLogger : MessagesLogger<ParsserMessageType> {


		public ParserMessagesLogger(MessagesCollection msgs) : base(msgs) {

		}

		public void LogMessage(ParsserMessageType msgType, Position pos, params object[] args) {
			logMessage(msgType, pos, args);
		}

		public override string GetMessageTypeId(ParsserMessageType msgType) {
			return ((int)msgType).ToString();
		}

		protected override string resolveMessage(ParsserMessageType msgType, out MessageType type, params object[] args) {

			type = MessageType.Error;

			switch (msgType) {

				case ParsserMessageType.UnrecognizedInput:
					return "Unrecognized input `{0}`.".Fmt(args);
				case ParsserMessageType.FailedParseInt:
					return "Failed to parse `{0}` as integer.".Fmt(args);
				case ParsserMessageType.FailedParseFloat:
					return "Failed to parse `{0}` as folating point number.".Fmt(args);
				case ParsserMessageType.UnterminatedMultilineComment:
					type = MessageType.Warning;
					return "Unterminated multiline comment";

				case ParsserMessageType.ConstDefInvalid:
					return "Constant definition is invalid.";
				case ParsserMessageType.ExcpectedParams:
					return "Parameters excpected.";
				case ParsserMessageType.FunStatementsInvalid:
					return "Function body is invalid.";
				case ParsserMessageType.LsystemHeaderInvalid:
					return "Lsystem name and optionally params excpected.";
				case ParsserMessageType.LsystemStatementsInvalid:
					return "Lsystem body is invalid.";
				case ParsserMessageType.SymbolsConstDefInvalid:
					return "Symbols constant definition is invalid.";
				case ParsserMessageType.SymbolsInterpretationInvalid:
					return "Symbols interpretation is invalid.";
				case ParsserMessageType.RewriteRuleInvalid:
					return "Rewrite rule is invalid.";
				case ParsserMessageType.RrReplacementInvalid:
					return "Rewrite rule replacement is invalid.";
				case ParsserMessageType.SymbolsPatternInvalid:
					return "Symbols pattern is invalid.";
				case ParsserMessageType.RrConstsDefInvalid:
					return "Constants definition is invalid.";
				case ParsserMessageType.RrConditionInvalid:
					return "Condition is invalid.";
				case ParsserMessageType.RrProbabilityInvalid:
					return "Probability is invalid.";
				case ParsserMessageType.OptionalParamsInvalid:
					return "Optional parameters are invalid.";
				case ParsserMessageType.EmptyExpression:
					return "Excpected non-empty expression.";
				case ParsserMessageType.ArrayExpressionInvalid:
					return "Array expression body is invalid.";



				case ParsserMessageType.EmptyRrConstsDefBlock:
					type = MessageType.Info;
					return "Empty constants definition block.";
				case ParsserMessageType.EmptyRrConditionBlock:
					type = MessageType.Info;
					return "Empty condition block.";
				case ParsserMessageType.EmptyRrReplacementBlock:
					type = MessageType.Info;
					return "Empty replacement block found, you can use keyword `nothing` to improve source code readability.";
				case ParsserMessageType.EmptyRrProbabilityBlock:
					type = MessageType.Info;
					return "Empty probability weight block.";

				default:
					return "Unknown error.";
			}
		}
	}

	public enum ParsserMessageType {
		Unknown,

		// lexer
		UnrecognizedInput,
		FailedParseInt,
		FailedParseFloat,
		UnterminatedMultilineComment,

		// parser
		ConstDefInvalid,
		ExcpectedParams,
		FunStatementsInvalid,
		LsystemHeaderInvalid,
		LsystemStatementsInvalid,
		SymbolsConstDefInvalid,
		SymbolsInterpretationInvalid,
		RewriteRuleInvalid,
		RrReplacementInvalid,
		SymbolsPatternInvalid,
		RrConstsDefInvalid,
		RrConditionInvalid,
		RrProbabilityInvalid,
		OptionalParamsInvalid,
		EmptyExpression,
		ArrayExpressionInvalid,


		EmptyRrConstsDefBlock,
		EmptyRrConditionBlock,
		EmptyRrReplacementBlock,
		EmptyRrProbabilityBlock,
	}
}

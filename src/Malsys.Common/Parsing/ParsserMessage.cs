
namespace Malsys.Parsing {
	public enum ParsserMessage {

		[Message(MessageType.Error, "Constant definition is invalid.")]
		ConstDefInvalid,
		[Message(MessageType.Error, "Parameters excepted.")]
		ExcpectedParams,
		[Message(MessageType.Error, "Function body is invalid.")]
		FunStatementsInvalid,
		[Message(MessageType.Error, "L-system name and optionally parameters excepted.")]
		LsystemHeaderInvalid,
		[Message(MessageType.Error, "L-system body is invalid.")]
		LsystemStatementsInvalid,
		[Message(MessageType.Error, "Symbols constant definition is invalid.")]
		SymbolsConstDefInvalid,
		[Message(MessageType.Error, "Symbols interpretation is invalid.")]
		SymbolsInterpretationInvalid,
		[Message(MessageType.Error, "Rewrite rule is invalid.")]
		RewriteRuleInvalid,
		[Message(MessageType.Error, "Rewrite rule replacement is invalid.")]
		RrReplacementInvalid,
		[Message(MessageType.Error, "Symbols pattern is invalid.")]
		SymbolsPatternInvalid,
		[Message(MessageType.Error, "Constants definition is invalid.")]
		RrConstsDefInvalid,
		[Message(MessageType.Error, "Condition is invalid.")]
		RrConditionInvalid,
		[Message(MessageType.Error, "Probability is invalid.")]
		RrProbabilityInvalid,
		[Message(MessageType.Error, "Optional parameters are invalid.")]
		OptionalParamsInvalid,
		[Message(MessageType.Error, "Excepted non-empty expression.")]
		EmptyExpression,
		[Message(MessageType.Error, "Array expression body is invalid.")]
		ArrayExpressionInvalid,
		[Message(MessageType.Error, "Process configuration body is invalid.")]
		ProcessConfigStatementsInvalid,
		[Message(MessageType.Error, "Missing default container type.")]
		ProcessConfigContainerMissingDefaultType,


		[Message(MessageType.Info, "Empty constants definition block.")]
		EmptyRrConstsDefBlock,
		[Message(MessageType.Info, "Empty condition block.")]
		EmptyRrConditionBlock,
		[Message(MessageType.Info, "Empty replacement block found, you can use keyword `nothing` to improve source code readability.")]
		EmptyRrReplacementBlock,
		[Message(MessageType.Info, "Empty probability weight block.")]
		EmptyRrProbabilityBlock,

	}
}

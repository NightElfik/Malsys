
namespace Malsys.SourceCode.Highlighters {
	public enum MarkType {
		Unknown,
		SingleLineComment,
		MultiLineComment,
		DocComment,

		Lsystem,
		LsystemName,

		VariableDefinition,
		VariableName,

		FunctionDefinition,
		FunctionName,

		ParameterName,

		RewriteRule,
		RrPattern,
		RrLeftCtxt,
		RrRightCtxt,
		RrCondition,
		RrProbability,

		Symbol,
		Keyword,
		Expression,

		MsgError,
		MsgWarning,
		MsgNotice,
	}
}


namespace Malsys.Expressions {
	public interface IPostfixExpressionMember {
		bool IsConstant { get; }
		bool IsArray { get; }
		bool IsVariable { get; }
		bool IsEvaluable { get; }
		bool IsUnknownFunction { get; }
	}
}

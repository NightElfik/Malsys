
namespace Malsys.Ast {

	/// <summary>
	/// All expression members should be immutable.
	/// </summary>
	public interface IExpressionMember : IToken, IExpressionVisitable {
	}

}

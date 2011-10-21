
namespace Malsys.Ast {
	public interface IToken : IAstVisitable {
		Position Position { get; }
	}
}

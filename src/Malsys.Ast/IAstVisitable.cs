
namespace Malsys.Ast {
	public interface IAstVisitable : IToken {
		void Accept(IAstVisitor visitor);
	}
}

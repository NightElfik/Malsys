
namespace Malsys.Ast {
	public interface IAstInputVisitable {
		void Accept(IAstInputVisitor visitor);
	}
}

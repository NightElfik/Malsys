
namespace Malsys.Ast {
	public interface IAstVisitable {
		void Accept(IAstVisitor visitor);
	}
}

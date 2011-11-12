
namespace Malsys.Ast {
	public interface IAstLsystemVisitable {
		void Accept(IAstLsystemVisitor visitor);
	}
}

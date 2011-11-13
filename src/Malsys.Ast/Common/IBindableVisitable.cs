
namespace Malsys.Ast {
	public interface IBindableVisitable {
		void Accept(IBindableVisitor visitor);
	}
}

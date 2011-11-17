
namespace Malsys.Ast {
	public interface IFunctionVisitable {

		void Accept(IFunctionVisitor visitor);

	}
}

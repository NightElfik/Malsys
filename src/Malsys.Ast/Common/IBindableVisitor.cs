
namespace Malsys.Ast {
	public interface IBindableVisitor {

		void Visit(Expression expr);
		void Visit(Function fun);
		void Visit(Lsystem lsystem);
		void Visit(LsystemSymbolList symbolsList);

	}
}

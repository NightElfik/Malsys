
namespace Malsys.Ast {
	public interface IAstInputVisitor {

		void Visit(Binding binding);
		void Visit(EmptyStatement emptyStat);
		void Visit(Lsystem lsystem);

	}
}

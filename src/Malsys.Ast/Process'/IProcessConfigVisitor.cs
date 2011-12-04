
namespace Malsys.Ast {
	public interface IProcessConfigVisitor {

		void Visit(EmptyStatement emptyStatement);
		void Visit(ProcessComponent component);
		void Visit(ProcessContainer container);
		void Visit(ProcessConfigConnection connection);

	}
}

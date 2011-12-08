
namespace Malsys.Ast {

	public interface IProcessConfigStatement : IStatement, IProcessConfigVisitable {

	}


	public interface IProcessConfigVisitable {

		void Accept(IProcessConfigVisitor visitor);

	}


	public interface IProcessConfigVisitor {

		void Visit(EmptyStatement emptyStatement);
		void Visit(ProcessComponent component);
		void Visit(ProcessContainer container);
		void Visit(ProcessConfigConnection connection);

	}
}

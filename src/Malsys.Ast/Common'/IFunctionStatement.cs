
namespace Malsys.Ast {

	public interface IFunctionStatement : IStatement, IFunctionVisitable {

	}


	public interface IFunctionVisitable {

		void Accept(IFunctionVisitor visitor);

	}


	public interface IFunctionVisitor {

		void Visit(ConstantDefinition constDef);
		void Visit(Expression expr);

	}

}

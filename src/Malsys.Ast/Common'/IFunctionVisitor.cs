
namespace Malsys.Ast {
	public interface IFunctionVisitor {

		void Visit(ConstantDefinition constDef);
		void Visit(Expression expr);

	}
}

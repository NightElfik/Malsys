
namespace Malsys.Ast {
	public interface IInputVisitor {

		void Visit(ConstantDefinition constDef);
		void Visit(EmptyStatement emptyStat);
		void Visit(FunctionDefinition funDef);
		void Visit(LsystemDefinition lsysDef);

	}
}

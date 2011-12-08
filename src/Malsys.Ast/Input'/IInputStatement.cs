
namespace Malsys.Ast {

	public interface IInputStatement : IStatement, IInputVisitable {

	}


	public interface IInputVisitable {

		void Accept(IInputVisitor visitor);

	}


	public interface IInputVisitor {

		void Visit(ConstantDefinition constDef);
		void Visit(EmptyStatement emptyStat);
		void Visit(FunctionDefinition funDef);
		void Visit(LsystemDefinition lsysDef);
		void Visit(ProcessConfigurationDefinition processConfDef);
		void Visit(ProcessStatement processStat);

	}

}


namespace Malsys.Ast {
	public interface ILsystemVisitor {

		void Visit(ConstantDefinition constDef);
		void Visit(EmptyStatement emptyStat);
		void Visit(FunctionDefinition funDef);
		void Visit(ProcessStatement processDef);
		void Visit(RewriteRule rewriteRule);
		void Visit(SymbolsInterpretDef symIntDef);
		void Visit(SymbolsConstDefinition symbolsDef);

	}
}

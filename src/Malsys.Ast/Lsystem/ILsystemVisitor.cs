
namespace Malsys.Ast {
	public interface ILsystemVisitor {

		void Visit(ConstantDefinition constDef);
		void Visit(EmptyStatement emptyStat);
		void Visit(FunctionDefinition funDef);
		void Visit(RewriteRule rewriteRule);
		void Visit(SymbolsInterpretDef symbolInterpretDef);
		void Visit(SymbolsConstDefinition symbolsDef);

	}
}

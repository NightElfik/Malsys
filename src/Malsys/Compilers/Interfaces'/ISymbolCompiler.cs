using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	public interface ISymbolCompiler :
			ICompiler<Ast.LsystemSymbol, Symbol<string>>,
			ICompiler<Ast.LsystemSymbol, Symbol<IExpression>> {

	}
}

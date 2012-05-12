/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	public interface ISymbolCompiler :
			ICompiler<Ast.LsystemSymbol, Symbol<string>>,
			ICompiler<Ast.LsystemSymbol, Symbol<IExpression>> {

	}
}

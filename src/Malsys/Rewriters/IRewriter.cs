using System.Collections.Generic;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Rewriters {
	public interface IRewriter : ISymbolProcessor {

		void Initialize(ISymbolProcessor outputProcessor, Dictionary<string, RewriteRule[]> rrules, VarMap vars, FunMap funs, int randomSeed);

	}
}

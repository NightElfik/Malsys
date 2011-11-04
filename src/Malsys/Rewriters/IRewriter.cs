using System;
using System.Collections.Generic;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using Symbol = Malsys.Symbol<Malsys.Expressions.IValue>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Rewriters {
	public interface IRewriter {

		void Initialize(Dictionary<string, RewriteRule[]> rrules, VarMap vars, FunMap funs, int randomSeed);

		void ReInitialize();

		IEnumerable<Symbol> Rewrite(IEnumerable<Symbol> source);

	}
}

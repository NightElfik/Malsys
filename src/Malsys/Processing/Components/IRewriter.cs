using System.Collections.Generic;
using Malsys.Expressions;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace Malsys.Processing.Components {
	public interface IRewriter : ISymbolProcessor {

		ISymbolProcessor OutputProcessor { set; }

		IEnumerable<RewriteRule> RewriteRules{ set; }

		VarMap Variables { set; }

		FunMap Functions { set; }

		[UserSettable]
		IValue RandomSeed { set; }

	}
}

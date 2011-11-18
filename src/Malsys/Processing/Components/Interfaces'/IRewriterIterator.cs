using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Expressions;
using Malsys.SemanticModel.Evaluated;
using Malsys.SemanticModel;

namespace Malsys.Processing.Components {
	public interface IRewriterIterator : ISymbolProcessor, IProcessStarter {

		IRewriter Rewriter { set; }

		ISymbolProcessor OutputProcessor { set; }

		[UserSettable]
		ImmutableList<Symbol<IValue>> Axiom { set; }

		[UserSettable]
		IValue Iterations { set; }

	}
}

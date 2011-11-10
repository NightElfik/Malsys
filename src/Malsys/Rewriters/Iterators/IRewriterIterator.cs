using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Expressions;

namespace Malsys.Rewriters.Iterators {
	public interface IRewriterIterator : ISymbolProcessor {

		IRewriter Rewriter { set; }

		ISymbolProcessor OutputProcessor { set; }

		[UserSettable]
		IEnumerable<Symbol<IValue>> Axiom { set; }

		[UserSettable]
		IValue Iterations { set; }


		void Start();
	}
}

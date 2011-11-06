using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Expressions;

namespace Malsys.Rewriters.Iterators {
	public interface IRewriterIterator {

		void Initialize(int iters, IRewriter rwter, ISymbolProcessor outProcessor, IEnumerable<Symbol<IValue>> symbols);


		void Start();
	}
}

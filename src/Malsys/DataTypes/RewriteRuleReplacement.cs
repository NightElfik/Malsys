using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Expressions;

namespace Malsys {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class RewriteRuleReplacement {

		public readonly SymbolsList<IExpression> Replacement;
		public readonly IExpression Weight;

		public RewriteRuleReplacement(IExpression wei, SymbolsList<IExpression> replac) {

		}
	}
}

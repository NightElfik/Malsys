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

		public static readonly RewriteRuleReplacement Empty
			= new RewriteRuleReplacement(SymbolsList<IExpression>.Empty, Constant.One);


		public readonly SymbolsList<IExpression> Replacement;
		public readonly IExpression Weight;

		public RewriteRuleReplacement(SymbolsList<IExpression> replac, IExpression wei) {
			Replacement = replac;
			Weight = wei;
		}
	}
}

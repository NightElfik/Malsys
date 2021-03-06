﻿using System.Collections.Generic;
using System.Linq;
using Malsys.Processing.Context;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Processing.Components.Rewriters {
	internal class RewriterRewriteRule {

		public Symbol<string> SymbolPattern;

		public ContextListNode<string>.ContextList LeftContext;

		public ContextListNode<string>.ContextList RightContext;
		/// <summary>
		/// Count of nodes in right context list.
		/// </summary>
		public int RightContextLenth;

		public IList<ConstantDefinition> LocalConstantDefs;

		public IExpression Condition;

		public IList<RewriteRuleReplacement> Replacements;


		public RewriterRewriteRule(ContextListNode<string>.ContextList leftContext, ContextListNode<string>.ContextList rightContext) {

			LeftContext = leftContext;
			RightContext = rightContext;

			RightContextLenth = RightContext.Count();

		}

	}
}

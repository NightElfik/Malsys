// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;

namespace Malsys.SemanticModel.Compiled {
	public class RewriteRule : ILsystemStatement {

		public Symbol<string> SymbolPattern;
		public List<Symbol<string>> LeftContext;
		public List<Symbol<string>> RightContext;
		public List<ConstantDefinition> LocalConstantDefs;
		public IExpression Condition;

		public List<RewriteRuleReplacement> Replacements;

		public readonly Ast.RewriteRule AstNode;


		public RewriteRule(Ast.RewriteRule astNode) {
			AstNode = astNode;
		}


		public LsystemStatementType StatementType {
			get { return LsystemStatementType.RewriteRule; }
		}

	}

}

// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;
using Malsys.SemanticModel.Compiled;

namespace Malsys.SemanticModel.Evaluated {
	public class SymbolInterpretationEvaled {

		public string Symbol;
		public List<OptionalParameterEvaled> Parameters;
		public string InstructionName;
		public List<IExpression> InstructionParameters;

		public bool InstructionIsLsystemName;
		public string LsystemConfigName;

		public Ast.SymbolsInterpretDef AstNode;


		public SymbolInterpretationEvaled(Ast.SymbolsInterpretDef astNode) {
			AstNode = astNode;
		}


	}
}

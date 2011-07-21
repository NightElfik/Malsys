using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Malsys.Ast {
	public class FunctionDefinition : IToken, IInputFileStatement, IExpressionInteractiveStatement {
		public readonly Keyword Keyword;
		public readonly Identificator Name;
		public readonly ReadOnlyCollection<OptionalParameter> Parameters;
		public readonly ReadOnlyCollection<VariableDefinition> VariableDefinitions;
		public readonly Expression Expression;

		public FunctionDefinition(Keyword keyword, Identificator name, IList<OptionalParameter> parameters, IList<VariableDefinition> varDefs, Expression expr, Position pos) {
			Keyword = keyword;
			Name = name;
			Parameters = new ReadOnlyCollection<OptionalParameter>(parameters);
			VariableDefinitions = new ReadOnlyCollection<VariableDefinition>(varDefs);
			Expression = expr;
			Position = pos;
		}

		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

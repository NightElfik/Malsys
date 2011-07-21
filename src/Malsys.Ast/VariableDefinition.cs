using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Malsys.Ast {

	public class VariableDefinition : IToken, IInputFileStatement, ILsystemStatement, IExpressionInteractiveStatement {
		public readonly Keyword Keyword;
		public readonly Identificator Name;
		public readonly IValue Value;

		public VariableDefinition(Keyword keyword, Identificator name, IValue value, Position pos) {
			Keyword = keyword;
			Name = name;
			Value = value;
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

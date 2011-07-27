using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Symbol : IToken, IAstVisitable {

		public readonly string Name;

		public Symbol(string name, Position pos) {
			Name = name;
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

	/// <summary>
	/// Immutable.
	/// </summary>
	public class SymbolPattern : Symbol, IAstVisitable {

		public readonly ImmutableList<Identificator> ParametersNames;


		public SymbolPattern(Symbol symbol, Position pos)
			: base(symbol.Name, pos) {

			ParametersNames = ImmutableList<Identificator>.Empty;
		}

		public SymbolPattern(Symbol symbol, IEnumerable<Identificator> paramsNames, Position pos)
			: base(symbol.Name, pos) {

			ParametersNames = new ImmutableList<Identificator>(paramsNames);
		}


		#region IAstVisitable Members

		public new void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}

	/// <summary>
	/// Immutable.
	/// </summary>
	public class SymbolExprArgs : Symbol, IAstVisitable {

		public readonly ImmutableList<Expression> Arguments;


		public SymbolExprArgs(Symbol symbol, Position pos)
			: base(symbol.Name, pos) {

			Arguments = ImmutableList<Expression>.Empty;
		}

		public SymbolExprArgs(Symbol symbol, IEnumerable<Expression> args, Position pos)
			: base(symbol.Name, pos) {

			Arguments = new ImmutableList<Expression>(args);
		}


		#region IAstVisitable Members

		public new void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

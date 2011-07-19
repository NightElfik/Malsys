using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Malsys.Ast {
	public class Symbol : IToken, IAstVisitable {
		public readonly string Syntax;

		public Symbol(string syntax, Position pos) {
			Syntax = syntax;
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

	public class SymbolPattern : IToken, IAstVisitable {
		public readonly Symbol Symbol;
		public readonly ReadOnlyCollection<Identificator> ParametersNames;

		public SymbolPattern(Symbol symbol, Position pos) {
			Symbol = symbol;
			ParametersNames = new ReadOnlyCollection<Identificator>(new Identificator[0]);
			Position = pos;
		}

		public SymbolPattern(Symbol symbol, IList<Identificator> paramsNames, Position pos) {
			Symbol = symbol;
			ParametersNames = new ReadOnlyCollection<Identificator>(paramsNames);
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

	public class SymbolWithParams : IToken, IAstVisitable {
		public readonly Symbol Symbol;
		public readonly ReadOnlyCollection<Expression> Parameters;

		public SymbolWithParams(Symbol symbol, Position pos) {
			Symbol = symbol;
			Parameters = new ReadOnlyCollection<Expression>(new Expression[0]);
			Position = pos;
		}

		public SymbolWithParams(Symbol symbol, IList<Expression> parameters, Position pos) {
			Symbol = symbol;
			Parameters = new ReadOnlyCollection<Expression>(parameters);
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

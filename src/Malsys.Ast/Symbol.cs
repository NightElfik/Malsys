using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Malsys.Ast {
	public class Symbol : Token, IAstVisitable {
		public readonly string Syntax;

		public Symbol(string syntax, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			Syntax = syntax;
		}

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}

	public class SymbolPattern : Token, IAstVisitable {
		public readonly Symbol Symbol;
		public readonly ReadOnlyCollection<Identificator> ParametersNames;

		public SymbolPattern(Symbol symbol, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			Symbol = symbol;
			ParametersNames = new ReadOnlyCollection<Identificator>(new Identificator[0]);
		}

		public SymbolPattern(Symbol symbol, IList<Identificator> paramsNames, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			Symbol = symbol;
			ParametersNames = new ReadOnlyCollection<Identificator>(paramsNames);
		}

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}

	public class SymbolWithParams : Token, IAstVisitable {
		public readonly Symbol Symbol;
		public readonly ReadOnlyCollection<Expression> Parameters;

		public SymbolWithParams(Symbol symbol, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			Symbol = symbol;
			Parameters = new ReadOnlyCollection<Expression>(new Expression[0]);
		}

		public SymbolWithParams(Symbol symbol, IList<Expression> parameters, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			Symbol = symbol;
			Parameters = new ReadOnlyCollection<Expression>(parameters);
		}

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}

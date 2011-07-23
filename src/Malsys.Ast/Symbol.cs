using System.Collections.Generic;
using System.Linq;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
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

	/// <summary>
	/// Immutable.
	/// </summary>
	public class SymbolPattern : IToken, IAstVisitable {

		public Identificator this[int i] { get { return parametersNames[i]; } }

		public readonly Symbol Symbol;
		public readonly int ParametersCount;

		private Identificator[] parametersNames;


		public SymbolPattern(Symbol symbol, Position pos) {
			Symbol = symbol;
			parametersNames = new Identificator[0];
			Position = pos;

			ParametersCount = parametersNames.Length;
		}

		public SymbolPattern(Symbol symbol, IEnumerable<Identificator> paramsNames, Position pos) {
			Symbol = symbol;
			parametersNames = paramsNames.ToArray();
			Position = pos;

			ParametersCount = parametersNames.Length;
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
	public class SymbolWithArgs : IToken, IAstVisitable {

		public Expression this[int i] { get { return arguments[i]; } }

		public readonly Symbol Symbol;
		public readonly int ArgumentsCount;

		private Expression[] arguments;


		public SymbolWithArgs(Symbol symbol, Position pos) {
			Symbol = symbol;
			arguments = new Expression[0];
			Position = pos;

			ArgumentsCount = arguments.Length;
		}

		public SymbolWithArgs(Symbol symbol, IList<Expression> args, Position pos) {
			Symbol = symbol;
			arguments = args.ToArray();
			Position = pos;

			ArgumentsCount = arguments.Length;
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

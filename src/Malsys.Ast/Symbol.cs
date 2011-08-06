using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Symbol : IToken {

		public readonly string Name;

		public Symbol(string name, Position pos) {
			Name = name;
			Position = pos;
		}

		#region IToken Members

		public Position Position { get; private set; }

		#endregion
	}

	/// <summary>
	/// Immutable.
	/// </summary>
	public class SymbolPattern : IToken {

		public readonly Symbol Symbol;

		public readonly ImmutableList<Identificator> ParametersNames;


		public SymbolPattern(Symbol symbol, Position pos) {
			Symbol = symbol;
			ParametersNames = ImmutableList<Identificator>.Empty;
			Position = pos;
		}

		public SymbolPattern(Symbol symbol, IEnumerable<Identificator> paramsNames, Position pos) {
			Symbol = symbol;
			ParametersNames = new ImmutableList<Identificator>(paramsNames);
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion
	}

	/// <summary>
	/// Immutable.
	/// </summary>
	public class SymbolExprArgs {

		public readonly Symbol Symbol;

		public readonly ImmutableList<Expression> Arguments;


		public SymbolExprArgs(Symbol symbol, Position pos) {
			Symbol = symbol;
			Arguments = ImmutableList<Expression>.Empty;
			Position = pos;
		}

		public SymbolExprArgs(Symbol symbol, IEnumerable<Expression> args, Position pos) {
			Symbol = symbol;
			Arguments = new ImmutableList<Expression>(args);
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion
	}
}

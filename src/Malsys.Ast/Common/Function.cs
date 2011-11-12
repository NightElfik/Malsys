using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Function : IBindable {

		public readonly ImmutableListPos<OptionalParameter> Parameters;
		public readonly ImmutableListPos<Binding> LocalBindings;
		public readonly Expression ReturnExpression;


		public Function(ImmutableListPos<OptionalParameter> prms, ImmutableListPos<Binding> bindings, Expression retExpr, Position pos) {

			Parameters = prms;
			LocalBindings = bindings;
			ReturnExpression = retExpr;

			Position = pos;
		}


		public int ParametersCount { get { return Parameters.Length; } }


		#region IToken Members

		public Position Position { get; private set; }

		#endregion
	}
}

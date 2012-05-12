/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <summary>
	/// Immutable singleton (quite unusual :).
	/// </summary>
	public class EmptyExpression : IExpression {

		public static readonly EmptyExpression Instance = new EmptyExpression();

		/// <summary>
		/// To avoid unnecessary use static instance instead.
		/// </summary>
		private EmptyExpression() { }


		public ExpressionType ExpressionType { get { return ExpressionType.EmptyExpression; } }

	}
}

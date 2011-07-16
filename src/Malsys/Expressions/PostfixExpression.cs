using System.Linq;
using System.Collections.Generic;
using System;

namespace Malsys.Expressions {
	/// <summary>
	/// Postfix expression is arithmetic expression in Reverse Polish notation (RPN).
	/// </summary>
	/// <remarks>
	/// It can contain members of type <c>double</c> (constant value), <c>string</c> (variable) and <see cref="ArithmeticFunction"/> (function).
	/// </remarks>
	public class PostfixExpression {
		#region Static members

		public static readonly PostfixExpression Empty = new PostfixExpression() { members = new object[0] };
		public static readonly PostfixExpression[] EmptyArray = new PostfixExpression[0];

		#endregion

		#region Properties

		public object this[int i] { get { return members[i]; } }

		public int Count { get { return members.Length; } }

		public bool IsEmpty { get { return members.Length == 0; } }

		#endregion


		private object[] members;


		#region Constructors

		private PostfixExpression() { }

		public PostfixExpression(params object[] members) {
			this.members = members;
		}

		public PostfixExpression(IList<object> members) {
			this.members = members.ToArray();
		}

		#endregion

		/// <summary>
		/// Evaluates postfix expression. If it contains any variable (<c>string</c>) throws <c>FormatException</c>.
		/// </summary>
		public double Evaluate() {
			Stack<double> stack = new Stack<double>(4);

			for (int i = 0; i < members.Length; i++) {
				object mbr = members[i];
				if (mbr is double) {
					stack.Push((double)mbr);
				}
				else if (mbr is string) {
					throw new FormatException("Unknown variable `{0}`.".Fmt(mbr));
				}
				else if (mbr is ArithmeticFunction) {
					ArithmeticFunction fun = (ArithmeticFunction)mbr;

					// TODO: improve arguments sending, maybe pool for double arrays or use some global array?
					// but keep thread safety
					double[] prms = new double[fun.Arity];
					for (int j = fun.Arity - 1; j >= 0; j--) {
						prms[j] = stack.Pop();
					}

					stack.Push(fun.Evaluate(prms));
				}
#if DEBUG
				else {
					throw new InvalidOperationException("Type `{0}` is not supported in postfix expression.".Fmt(mbr.GetType().Name));
				}
#endif
			}

			if (stack.Count != 1) {
				throw new FormatException("Too much operands to evaluate expression.");
			}

			return stack.Pop();
		}
	}
}

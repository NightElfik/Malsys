using System;
using System.Collections.Generic;
using System.Linq;
using Malsys.Expressions;

namespace Malsys {
	/// <summary>
	/// Postfix expression is arithmetic expression in Reverse Polish notation (RPN).
	/// </summary>
	public class PostfixExpression : IVariableValue {
		#region Static members

		public static readonly PostfixExpression Empty = new PostfixExpression() { members = new IPostfixExpressionMember[0] };
		public static readonly PostfixExpression[] EmptyArray = new PostfixExpression[0];

		#endregion

		#region Properties

		public object this[int i] { get { return members[i]; } }

		public int Count { get { return members.Length; } }

		public bool IsEmpty { get { return members.Length == 0; } }

		#endregion


		private IPostfixExpressionMember[] members;


		#region Constructors

		private PostfixExpression() { }

		public PostfixExpression(params IPostfixExpressionMember[] members) {
			this.members = members;
		}

		public PostfixExpression(IList<IPostfixExpressionMember> members) {
			this.members = members.ToArray();
		}

		#endregion

		/// <summary>
		/// Evaluates postfix expression.
		/// </summary>
		public IPostfixExpressionMember Evaluate() {
			Stack<IPostfixExpressionMember> stack = new Stack<IPostfixExpressionMember>(4);

			/*for (int i = 0; i < members.Length; i++) {
				object mbr = members[i];
				if (mbr is double) {
					stack.Push((double)mbr);
				}
				else if (mbr is string) {
					throw new FormatException("Unknown variable `{0}`.".Fmt(mbr));
				}
				else if (mbr is IEvaluable) {
					IEvaluable evaluable = (IEvaluable)mbr;

					// TODO: improve arguments sending, maybe pool for double arrays or use some global array?
					// but keep thread safety
					double[] prms = new double[evaluable.Arity];
					for (int j = evaluable.Arity - 1; j >= 0; j--) {
						prms[j] = stack.Pop();
					}

					stack.Push(evaluable.Evaluate(prms));
				}
#if DEBUG
				else {
					throw new InvalidOperationException("Type `{0}` is not supported in postfix expression.".Fmt(mbr.GetType().Name));
				}
#endif
			}

			if (stack.Count != 1) {
				throw new FormatException("Too much operands to evaluate expression.");
			}*/

			return stack.Pop();
		}
	}
}

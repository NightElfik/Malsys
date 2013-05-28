// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;
using System.Text;

namespace Malsys.SemanticModel.Evaluated {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ValuesArray : ImmutableList<IValue>, IValue {

		new public static readonly ValuesArray Empty = new ValuesArray((Ast.ExpressionsArray)null);



		public readonly Ast.ExpressionsArray AstNode;


		public ValuesArray(Ast.ExpressionsArray astNode = null)
			: base(ImmutableList<IValue>.Empty) {

				AstNode = astNode;
		}

		public ValuesArray(IEnumerable<IValue> vals, Ast.ExpressionsArray astNode = null)
			: base(vals) {

			AstNode = astNode;
		}


		public ValuesArray(ImmutableList<IValue> vals, Ast.ExpressionsArray astNode = null)
			: base(vals) {

			AstNode = astNode;
		}

		public ValuesArray(params IValue[] vals)
			: base(vals) {

			AstNode = null;
		}


		public bool IsConstant { get { return false; } }

		public bool IsArray { get { return true; } }

		public bool IsNaN { get { return false; } }

		public ExpressionValueType Type { get { return ExpressionValueType.Array; } }

		public PositionRange AstPosition { get { return AstNode == null ? PositionRange.Unknown : AstNode.Position; } }



		public void ToString(StringBuilder sb) {

			sb.Append("{");

			for (int i = 0; i < Length; i++) {
				if (i != 0) {
					sb.Append(", ");
				}

				sb.Append(this[i].ToString());
			}

			sb.Append("}");
		}

		public override string ToString() {

			var sb = new StringBuilder();
			ToString(sb);
			return sb.ToString();
		}


		public int CompareTo(IValue other) {
			if (other.IsConstant) {
				return 1; // array is more than constant
			}
			else {
				ValuesArray o = (ValuesArray)other;
				int cmp = Length.CompareTo(o.Length);

				if (cmp == 0) {
					// arrays have the same length
					for (int i = 0; i < Length; i++) {
						cmp = this[i].CompareTo(o[i]);
						if (cmp != 0) {
							return cmp;  // values at index i are first different
						}
					}

					return 0;  // they are the same
				}
				else {
					return cmp;
				}
			}
		}


	}
}

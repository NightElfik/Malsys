using System.Collections.Generic;
using System.Text;

namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ValuesArray : ImmutableList<IValue>, IValue {

		new public static readonly ValuesArray Empty = new ValuesArray();


		public ValuesArray()
			: base(ImmutableList<IValue>.Empty) { }

		public ValuesArray(IEnumerable<IValue> vals)
			: base(vals) { }


		public ValuesArray(ImmutableList<IValue> vals)
			: base(vals) { }


		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.Append("{");

			for (int i = 0; i < Length; i++) {
				if (i != 0) {
					sb.Append(", ");
				}

				sb.Append(this[i].ToString());
			}

			sb.Append("}");
			return sb.ToString();
		}


		#region IValue Members

		public bool IsConstant { get { return false; } }
		public bool IsArray { get { return true; } }
		public ExpressionValueType Type { get { return ExpressionValueType.Array; } }

		#endregion

		#region IComparable<IArithmeticValue> Members

		public int CompareTo(IValue other) {
			if (other.IsConstant) {
				return 1; // array is more than constant
			}
			else {
				ValuesArray o = (ValuesArray)other;
				int cmp = Length.CompareTo(o.Length);

				if (cmp == 0) {
					// arrays have same length
					for (int i = 0; i < Length; i++) {
						cmp = this[i].CompareTo(o[i]);
						if (cmp != 0) {
							return cmp;  // values at index i are first different
						}
					}

					return 0; // they are same
				}
				else {
					return cmp;
				}
			}
		}

		#endregion
	}
}

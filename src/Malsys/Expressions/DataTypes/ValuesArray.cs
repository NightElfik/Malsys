using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ValuesArray : IValue {

		public static ValuesArray Empty = new ValuesArray(new IValue[0]);


		public IValue this[int i] { get { return values[i]; } }
		public int Length { get { return values.Length; } }

		private IValue[] values;


		public ValuesArray(IEnumerable<IValue> vals) {
			values = vals.ToArray();
		}

		/// <summary>
		/// Faster version of constructing ValuesArray.
		/// Use only if no other reference will exist on given array.
		/// Do not copy elements from given array, just takes its reference.
		/// </summary>
		internal ValuesArray(IValue[] immutableVals) {
			values = immutableVals;
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.Append("{");

			for (int i = 0; i < values.Length; i++) {
				if (i != 0) {
					sb.Append(", ");
				}

				sb.Append(values[i].ToString());
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
				int cmp = values.Length.CompareTo(o.values.Length);

				if (cmp == 0) {
					// arrays have same length
					for (int i = 0; i < values.Length; i++) {
						cmp = values[i].CompareTo(o.values[i]);
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

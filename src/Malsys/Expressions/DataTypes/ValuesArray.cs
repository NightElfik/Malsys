using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ValuesArray : IValue {

		public IValue this[int i] { get { return values[i]; } }
		public int Length { get { return values.Length; } }

		private IValue[] values;


		public ValuesArray(IList<IValue> values) {
			this.values = new IValue[values.Count];
			for (int i = 0; i < values.Count; i++) {
				this.values[i] = values[i];
			}
		}

		/// <summary>
		/// Faster version of constructing ValuesArray.
		/// Use only if no other reference will exist on given array.
		/// Do not copy elements from given array, just takes its reference.
		/// </summary>
		internal ValuesArray(IValue[] immutableValues) {
			values = immutableValues;
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

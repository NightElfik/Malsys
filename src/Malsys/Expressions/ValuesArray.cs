
namespace Malsys.Expressions {
	public class ValuesArray : IValue {
		public IValue[] Values;

		#region IArithmeticValue Members

		public bool IsConstant { get { return false; } }
		public bool IsArray { get { return true; } }
		public IArithmeticValueType Type { get { return IArithmeticValueType.Array; } }

		#endregion

		#region IComparable<IArithmeticValue> Members

		public int CompareTo(IValue other) {
			if (other.IsConstant) {
				return 1; // array is more than constant
			}
			else {
				ValuesArray o = (ValuesArray)other;
				int cmp = Values.Length.CompareTo(o.Values.Length);

				if (cmp == 0) {
					// arrays have same length
					for (int i = 0; i < Values.Length; i++) {
						cmp = Values[i].CompareTo(o.Values[i]);
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

using System.Collections.Generic;

namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExpressionValuesArray : IExpressionValue, IPostfixExpressionMember {

		public IExpressionValue this[int i] { get { return values[i]; } }
		public int Length { get { return values.Length; } }

		private IExpressionValue[] values;


		public ExpressionValuesArray(IList<IExpressionValue> values) {
			this.values = new IExpressionValue[values.Count];
			for (int i = 0; i < values.Count; i++) {
				this.values[i] = values[i];
			}
		}

		/// <summary>
		/// Faster version of constructing ExpressionValuesArray.
		/// Use only if no other reference will exist on given array.
		/// Do not copy elements from given array, just takes its reference.
		/// </summary>
		internal ExpressionValuesArray(IExpressionValue[] immutableValues) {
			values = immutableValues;
		}


		#region IExpressionValue Members

		public bool IsExpression { get { return false; } }
		public bool IsArray { get { return true; } }

		#endregion

		#region IPostfixExpressionMember Members

		public bool IsConstant { get { return false; } }
		//public bool IsArray { get { return true; } }
		public bool IsVariable { get { return false; } }
		public bool IsEvaluable { get { return false; } }
		public bool IsUnknownFunction { get { return false; } }

		#endregion
	}
}

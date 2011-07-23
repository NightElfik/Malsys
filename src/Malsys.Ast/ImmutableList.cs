using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Malsys.Ast {
	public class ImmutableList<T> : IToken , IEnumerable<T> {

		public T this[int i] { get { return values[i]; } }
		public readonly int Length;

		private T[] values;


		public ImmutableList(Position pos) {
			values = new T[0];
			Length = values.Length;
		}

		public ImmutableList(IEnumerable<T> vals, Position pos) {
			values = vals.ToArray();
			Length = values.Length;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			return values.GetEnumerator();
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator() {
			return ((IEnumerable<T>)values).GetEnumerator();
		}

		#endregion
	}
}

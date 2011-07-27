using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Malsys {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ImmutableList<T> : IList<T> {

		public static readonly ImmutableList<T> Empty = new ImmutableList<T>();


		public readonly int Length;

		private T[] values;


		private ImmutableList() {
			values = new T[0];
		}

		public ImmutableList(IEnumerable<T> vals) {
			values = vals.ToArray();
			Length = values.Length;
		}

		public ImmutableList(IList<T> vals) {

			Length = vals.Count;

			if (Length == 0) {
				values = Empty.values;
			}
			else {
				values = new T[Length];

				for (int i = 0; i < Length; i++) {
					values[i] = vals[i];
				}
			}
		}

		public ImmutableList(ImmutableList<T> vals) {
			values = vals.values;
			Length = values.Length;
		}

		/// <summary>
		/// Fast constructor of immutable list if caller knows, that given list has no other references
		/// and is unnecessary to copy all elements into new 'private' array.
		/// Do not use if you don't understand it!
		/// </summary>
		public ImmutableList(T[] vals, bool noOtherReferenceExistsOnGivenValuesArray) {
			Length = vals.Length;
			if (noOtherReferenceExistsOnGivenValuesArray) {
				values = vals;
			}
			else {
				values = new T[Length];
				for (int i = 0; i < Length; i++) {
					values[i] = vals[i];
				}
			}
		}


		#region IList<T> Members

		public int IndexOf(T item) {
			return Array.IndexOf<T>(values, item);
		}

		public void Insert(int index, T item) {
			throw new NotSupportedException();
		}

		public void RemoveAt(int index) {
			throw new NotSupportedException();
		}

		public T this[int index] {
			get {
				return values[index];
			}
			set {
				throw new NotSupportedException();
			}
		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item) {
			throw new NotSupportedException();
		}

		public void Clear() {
			throw new NotSupportedException();
		}

		public bool Contains(T item) {
			throw new NotImplementedException();
		}

		public void CopyTo(T[] array, int arrayIndex) {
			throw new NotImplementedException();
		}

		public int Count {
			get { return Length; }
		}

		public bool IsReadOnly {
			get { return true; }
		}

		public bool Remove(T item) {
			throw new NotSupportedException();
		}

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

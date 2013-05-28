// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Malsys {
	/// <summary>
	/// Immutable generic list of values.
	/// </summary>
	public class ImmutableList<T> : IList<T> {

		public static readonly ImmutableList<T> Empty = new ImmutableList<T>();


		public readonly int Length;

		private T[] values;


		#region Constructors

		private ImmutableList() {
			Length = 0;
			values = new T[0];
		}

		private ImmutableList(T[] vals, int dummy) {
			Length = vals.Length;
			values = vals;
		}


		public ImmutableList(IEnumerable<T> vals) {

			Debug.Assert(!(vals is ImmutableList<T>), "Bad constructor of {0} used, probably somewhere missing ctor with {0} and it was downcasted to {1}."
					.Fmt(typeof(ImmutableList<T>).Name, typeof(IEnumerable<T>).Name));

			values = vals.ToArray();
			Length = values.Length;
		}

		public ImmutableList(IList<T> vals) {

			Debug.Assert(!(vals is ImmutableList<T>), "Bad constructor of {0} used, probably somewhere missing ctor with {0} and it was downcasted to {1}."
					.Fmt(typeof(ImmutableList<T>).Name, typeof(IList<T>).Name));

			Length = vals.Count;

			if (Length == 0) {
				values = Empty.values;
			}
			else {
				values = new T[Length];
				vals.CopyTo(values, 0);
			}
		}

		public ImmutableList(params T[] vals)
			: this((IList<T>)vals) {
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

		#endregion


		public bool IsEmpty { get { return Length == 0; } }


		public ImmutableList<TResult> Convert<TResult>(Func<T, TResult> convertFunc) {

			TResult[] resultArr = new TResult[Length];

			for (int i = 0; i < Length; i++) {
				resultArr[i] = convertFunc(values[i]);
			}

			return new ImmutableList<TResult>(resultArr, 0);
		}


		public ImmutableList<T> AddRange(ImmutableList<T> addedVals) {

			if (Length == 0) {
				return addedVals;
			}

			if (addedVals.Length == 0) {
				return this;
			}

			int len = Length + values.Length;

			var newVals = new T[len];
			Array.Copy(values, newVals, Length);
			Array.Copy(addedVals.values, 0, newVals, Length, addedVals.Length);

			return new ImmutableList<T>(newVals, 0);
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
			return values.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex) {
			Array.Copy(values, 0, array, arrayIndex, Length);
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

		#region IEnumerable<T> Members

		IEnumerator IEnumerable.GetEnumerator() {
			return values.GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator() {
			return ((IEnumerable<T>)values).GetEnumerator();
		}

		#endregion

	}


	public static class ImmutableListExtensions {

		public static ImmutableList<T> ToImmutableList<T>(this IList<T> list) {
			return new ImmutableList<T>(list);
		}

		public static ImmutableList<T> ToImmutableList<T>(this IEnumerable<T> list) {
			return new ImmutableList<T>(list);
		}

	}
}

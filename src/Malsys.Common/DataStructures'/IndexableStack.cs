/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;

namespace Malsys {
	/// <summary>
	/// Dynamic stack that can be indexed (from top).
	/// </summary>
	public class IndexableStack<T> {

		private const int defaultCapacity = 32;
		private const float defaultGrowFactor = 2.0f;
		private const int minimumGrow = 8;

		/// <summary>
		/// Storage for items.
		/// </summary>
		private T[] data;
		/// <summary>
		/// Number of elements.
		/// </summary>
		private int size;
		/// <summary>
		/// 100-times grow factor (200 = 2.0).
		/// </summary>
		private int growFactor;


		#region Constructors

		/// <param name="initialCapacity">Initial capacity of the stack.</param>
		/// <param name="growFactor">The old capacity is multiplied by the growFactor while extending the capacity of the stack.</param>
		public IndexableStack(int initialCapacity = defaultCapacity, float growFactor=defaultGrowFactor) {
			if (initialCapacity < 0) {
				throw new ArgumentOutOfRangeException("initialCapacity", "Capacity should be greater than zero.");
			}
			if (growFactor < 1.0f || growFactor > 8.0f) {
				throw new ArgumentOutOfRangeException("growFactor", "Grow factor should be between 1.0 and 8.0.");
			}

			data = new T[initialCapacity];
			this.growFactor = (int)(growFactor * 100);
			size = 0;
		}

		#endregion


		#region Properties and indexers

		/// <summary>
		/// Returns i-th (zero-based) item from the top of the stack.
		/// </summary>
		public T this[int i] {
			get {
				if (i < 0 || i >= size) {
					throw new ArgumentOutOfRangeException("i", "Zero based index ({0}) should be lower than stack size ({1}).".Fmt(i, size));
				}
				return data[i];
			}
		}

		/// <summary>
		/// Number of stored items in the stack.
		/// </summary>
		public int Count {
			get { return size; }
		}

		/// <summary>
		/// Total capacity of the stack.
		/// If the Count and Capacity are the same values the next addition will extend the stack by the grow factor.
		/// </summary>
		public int Capacity {
			get { return data.Length; }
		}


		#endregion


		#region Public methods

		/// <summary>
		/// Returns an item from the top of the stack without removing it.
		/// </summary>
		public T Peek() {
			if (size == 0) {
				throw new InvalidOperationException("Peek on empty stack.");
			}

			return data[size - 1];
		}

		/// <summary>
		/// Returns and removes an item from the top of the stack.
		/// </summary>
		public T Pop() {
			if (size == 0) {
				throw new InvalidOperationException("Pop from empty stack.");
			}

			T item = data[--size];
			data[size] = default(T);  // free memory quicker
			return item;
		}

		/// <summary>
		/// Adds given item to the top of the stack.
		/// </summary>
		public void Push(T item) {
			if (size == data.Length) {
				int newCapacity = (int)((long)data.Length * (long)growFactor / 100);

				if (newCapacity < data.Length + minimumGrow) {
					newCapacity = data.Length + minimumGrow;
				}

				setCapacity(newCapacity);
			}

			data[size++] = item;
		}

		#endregion


		#region Private methods

		private void setCapacity(int capacity) {
			T[] newData = new T[capacity];
			Array.Copy(data, 0, newData, 0, size);
			data = newData;
		}

		#endregion
	}
}

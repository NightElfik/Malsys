using System;

namespace Malsys.Common.DataTypes {

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

		public IndexableStack()
			: this(defaultCapacity, defaultGrowFactor) {
		}

		public IndexableStack(int initialCapacity)
			: this(initialCapacity, defaultGrowFactor) {
		}

		public IndexableStack(int initialCapacity, float growFactor) {
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

		public T this[int i] {
			get {
				if (i < 0 || i >= size) {
					throw new ArgumentOutOfRangeException("i", "Zero based index ({0}) should be lower than stack size ({1}).".Fmt(i, size));
				}
				return data[i];
			}
		}

		public int Count {
			get { return size; }
		}

		#endregion


		#region Public methods

		public T Peek() {
			if (size == 0) {
				throw new InvalidOperationException("Peek on empty stack.");
			}

			return data[size - 1];
		}

		public T Pop() {
			if (size == 0) {
				throw new InvalidOperationException("Pop from empty stack.");
			}

			T item = data[--size];
			data[size] = default(T);  // free memory quicker
			return item;
		}

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

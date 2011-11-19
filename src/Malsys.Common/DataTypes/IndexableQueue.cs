using System;
using System.Diagnostics.Contracts;

namespace Malsys {
	public class IndexableQueue<T> {

		private const int defaultCapacity = 32;
		private const float defaultGrowFactor = 2.0f;
		private const int minimumGrow = 8;


		/// <summary>
		/// Storage for items.
		/// </summary>
		private T[] data;

		/// <summary>
		/// Index of first valid element in the queue.
		/// </summary>
		private int head;

		/// <summary>
		/// Index after last valid element in the queue.
		/// </summary>
		private int tail;

		/// <summary>
		/// Number of elements.
		/// </summary>
		[ContractPublicPropertyName("Count")]
		private int size;

		/// <summary>
		/// 100-times grow factor (200 = 2.0).
		/// </summary>
		private int growFactor;


		#region Constructors

		public IndexableQueue()
			: this(defaultCapacity, defaultGrowFactor) {
		}

		public IndexableQueue(int initialCapacity)
			: this(initialCapacity, defaultGrowFactor) {
		}

		public IndexableQueue(int initialCapacity, float growFactor) {

			Contract.Requires<ArgumentOutOfRangeException>(initialCapacity >= 0);
			Contract.Requires<ArgumentOutOfRangeException>(growFactor >= 1.0f && growFactor <= 8.0f);

			data = new T[initialCapacity];
			this.growFactor = (int)(growFactor * 100);
			head = 0;
			tail = 0;
			size = 0;
		}

		#endregion


		#region Properties and indexers

		public T this[int i] {
			get {
				Contract.Requires<ArgumentOutOfRangeException>(i >= 0 && i < size);
				return data[(head + i) % data.Length];
			}
		}

		public int Count {
			get { return size; }
		}

		public int Capacity {
			get { return data.Length; }
		}

		#endregion


		#region Public methods

		public T Peek() {

			Contract.Requires<InvalidOperationException>(size > 0);

			return data[head];
		}

		public void Enqueue(T item) {
			if (size == data.Length) {
				int newCapacity = (int)((long)data.Length * (long)growFactor / 100);

				if (newCapacity < data.Length + minimumGrow) {
					newCapacity = data.Length + minimumGrow;
				}

				setCapacity(newCapacity);
			}

			data[tail] = item;
			tail = (tail + 1) % data.Length;
			size++;
		}

		public T Dequeue() {

			Contract.Requires<InvalidOperationException>(size > 0);

			T item = data[head];

			data[head] = default(T);
			head = (head + 1) % data.Length;
			size--;

			return item;
		}

		public void Clear() {
			if (head < tail) {
				Array.Clear(data, head, size);
			}
			else {
				Array.Clear(data, head, data.Length - head);
				Array.Clear(data, 0, tail);
			}

			head = 0;
			tail = 0;
			size = 0;
		}

		#endregion


		#region Private methods

		private void setCapacity(int capacity) {
			T[] newData = new T[capacity];
			if (size > 0) {
				if (head < tail) {
					Array.Copy(data, head, newData, 0, size);
				}
				else {
					Array.Copy(data, head, newData, 0, data.Length - head);
					Array.Copy(data, 0, newData, data.Length - head, tail);
				}
			}

			data = newData;
			head = 0;
			tail = (size == capacity) ? 0 : size;
		}

		#endregion
	}
}

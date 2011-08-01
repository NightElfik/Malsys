using System;

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
		private int size;
		/// <summary>
		/// 100-times grow factor (200 = 2.0).
		/// </summary>
		private int growFactor;


		public IndexableQueue()
			: this(defaultCapacity, defaultGrowFactor) {
		}

		public IndexableQueue(int capacity)
			: this(capacity, defaultGrowFactor) {
		}

		public IndexableQueue(int capacity, float growFactor) {
			if (capacity < 0) {
				throw new ArgumentOutOfRangeException("capacity", "Capacity should be greater than zero.");
			}
			if (growFactor < 1.0f || growFactor > 8.0f) {
				throw new ArgumentOutOfRangeException("growFactor", "Grow factor should be between 1.0 and 8.0.");
			}

			data = new T[capacity];
			this.growFactor = (int)(growFactor * 100);
			head = 0;
			tail = 0;
			size = 0;
		}


		public T this[int i] {
			get {
				if (i < 0 || i >= size) {
					throw new ArgumentOutOfRangeException("i", "Zero based index should be lower than queue size.");
				}
				return data[(head + i) % data.Length];
			}
		}

		public int Count {
			get { return size; }
		}



		public T Peek() {
			if (size == 0) {
				throw new InvalidOperationException("Peek on empty queue.");
			}

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
			if (size == 0) {
				throw new InvalidOperationException("Dequeue from empty queue.");
			}

			T item = data[head];

			data[head] = default(T);
			head = (head + 1) % data.Length;
			size--;

			return item;
		}



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
	}
}

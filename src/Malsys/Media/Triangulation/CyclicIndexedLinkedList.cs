using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Malsys.Media.Triangulation {
	/// <summary>
	/// Cyclic linked list. Members can be accessed also with index.
	/// </summary>
	public class CyclicIndexedLinkedList<T> {

		private Node entryNode;
		private Node[] nodes;
		private IList<T> data;


		public CyclicIndexedLinkedList(IList<T> items) {

			Count = items.Count;
			AllNodesCount = items.Count;
			nodes = new Node[Count];
			data = items;

			for (int i = 0; i < Count; i++) {
				nodes[i] = new Node(this, i);
			}

			entryNode = Count == 0 ? null : nodes[0];

			for (int i = 0; i < Count; i++) {
				nodes[i].Previous = nodes[(i - 1 + Count) % Count];
				nodes[i].Next = nodes[(i + 1) % Count];
			}
		}


		public Node this[int index] { get { return nodes[index]; } }

		public int Count { get; private set; }

		public int AllNodesCount { get; private set; }

		public Node EntryNode {
			get { return entryNode; }
			set {
				Contract.Requires<ArgumentException>(value.Parent == this);
				entryNode = value;
			}
		}

		public T GetData(int index) {
			return data[index];
		}


		public class Node {
			public CyclicIndexedLinkedList<T> Parent;
			public int Index;
			public T Item {
				get { return Parent.data[Index]; }
				set { Parent.data[Index] = value; }
			}
			public Node Previous;
			public Node Next;

			public Node(CyclicIndexedLinkedList<T> parent, int index) {
				Parent = parent;
				Index = index;
			}

			public void Remove() {
				Contract.Requires(Parent != null);
				Contract.Requires(Previous != null);
				Contract.Requires(Next != null);

				if (Parent.entryNode == this) {
					Parent.entryNode = Next;
				}

				// re-link neighbors
				Previous.Next = Next;
				Next.Previous = Previous;

				// un-link
				Previous = null;
				Next = null;

				Parent.Count--;
				Parent = null;
			}
		}

	}
}

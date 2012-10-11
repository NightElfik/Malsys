/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Malsys.SemanticModel;

namespace Malsys.Processing.Context {
	/// <summary>
	/// Linked list node which can hold symbol or another list.
	/// </summary>
	public class ContextListNode<T> {

		public ContextList ParentList { get { return parentList; } }
		private ContextList parentList;

		public ContextListNode<T> ParentNode { get { return parentList == null ? null : parentList.ParentNode; } }

		public ContextListNode<T> Previous { get; private set; }
		public ContextListNode<T> Next { get; private set; }

		public Symbol<T> Symbol { get; private set; }

		public ContextList InnerList { get; private set; }

		/// <summary>
		/// ID to identify whether this node was matched by current context search or not.
		/// </summary>
		/// <remarks>
		///	This property could be bool but then it would be necessary to reset
		///	all flags to false after context matching.
		///
		/// Instead, every context search should have its "unique" (incremental) ID
		/// and if ID id search is the same as ID of node, then it was visited.
		/// No resets are required after matching because next search will have different ID.
		/// </remarks>
		public int MatchedId { get; set; }


		/// <summary>
		/// Creates list item with empty inner list.
		/// </summary>
		public ContextListNode() {
			InnerList = new ContextList(this);
		}

		/// <summary>
		/// Creates list item with given item.
		/// </summary>
		public ContextListNode(Symbol<T> item) {
			Symbol = item;
			InnerList = null;
		}


		public bool IsSymbolNode { get { return InnerList == null; } }
		public bool IsListNode { get { return InnerList != null; } }


		public ContextListNode<T> GetNextNodeInHierarchy() {

			if (IsListNode && !InnerList.IsEmpty) {
				return InnerList.First;
			}

			if (Next != null) {
				return Next;
			}

			// no direct successor, we have to look in parents

			var nextNode = this;

			do {
				if (nextNode.ParentNode == null) {
					return null;
				}
				nextNode = nextNode.ParentNode;
			} while (nextNode.Next == null);

			return nextNode.Next;
		}

		public ContextListNode<T> GetNextSymbolNodeInHierarchy() {

			var node = this;
			do {
				node = node.GetNextNodeInHierarchy();
			} while (node != null && !node.IsSymbolNode);

			return node;
		}


		public class ContextList : IEnumerable<ContextListNode<T>> {

			public ContextListNode<T> ParentNode { get; private set; }

			public ContextListNode<T> First { get; private set; }
			public ContextListNode<T> Last { get; private set; }


			public ContextList(ContextListNode<T> parent) {
				ParentNode = parent;
			}


			public bool IsEmpty { get { return First == null; } }

			public bool IsCompleted { get; private set; }


			public ContextListNode<T> Append(ContextListNode<T> node) {

				node.parentList = this;

				if (Last != null) {
					Debug.Assert(Last.Next == null);
					Last.Next = node;
					node.Previous = Last;
				}
				else {
					First = node;
				}

				Last = node;
				return node;
			}


			public void Complete() {
				IsCompleted = true;
			}

			public void Clear() {
				First = null;
				Last = null;
				IsCompleted = false;
			}


			#region IEnumerable<ContextListNode<T>> Members

			public IEnumerator<ContextListNode<T>> GetEnumerator() {
				return new Enumerator(First);
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return new Enumerator(First);
			}

			#endregion


			private class Enumerator : IEnumerator<ContextListNode<T>> {

				private ContextListNode<T> first;
				private ContextListNode<T> currNode;

				public Enumerator(ContextListNode<T> first) {
					this.first = first;
					currNode = null;
				}


				public ContextListNode<T> Current {
					get { return currNode; }
				}

				object System.Collections.IEnumerator.Current {
					get { return currNode; }
				}

				public bool MoveNext() {
					if (currNode == null) {
						currNode = first;
						first = null;
					}
					else {
						currNode = currNode.Next;
					}
					return currNode != null;
				}

				public void Reset() {
					throw new NotImplementedException();
				}

				public void Dispose() {
					first = null;
					currNode = null;
				}


			}

		}

	}

}

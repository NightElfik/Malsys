using System;
using System.Collections;
using System.Collections.Generic;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	/// <summary>
	/// Argument storage to aviod creating small arrays to supply arguments for function invokes.
	/// </summary>
	public class ArgsStorage : IEnumerable<IValue> {

		public int ArgsCount { get; private set; }

		public IValue this[int i] { get { return args[i]; } }

		private IValue[] args = new IValue[8];


		public ArgsStorage() {
			Clear();
		}


		public bool IsEmpty { get { return ArgsCount == 0; } }


		/// <summary>
		/// Cleares all previously stored arguments and takes (pops) given number of new arguments from given stack.
		/// Top of stack is last agrument.
		/// Do not check weather is enough items in stack.
		/// </summary>
		public void PopArgs(int argsCount, Stack<IValue> stack) {

			ArgsCount = argsCount;
			ensureCapacity(ArgsCount);

			for (int i = argsCount - 1; i >= 0; i--) {
				args[i] = stack.Pop();
			}
		}

		/// <summary>
		/// Cleares all previously stored arguments and copies all args from first list and if second is longer,
		/// rest from second.
		/// </summary>
		public void AddArgs(ImmutableList<IValue> arguments, ImmutableList<IValue> defaultValues) {

			ArgsCount = Math.Max(arguments.Length, defaultValues.Length);
			ensureCapacity(ArgsCount);

			int i = 0;

			for (; i < arguments.Length; i++) {
				args[i] = arguments[i];
			}

			for (; i < defaultValues.Length; i++) {
				args[i] = defaultValues[i];
			}
		}

		public void Clear() {
			ArgsCount = 0;
		}


		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			return new Enumerator(this);
		}

		#endregion

		#region IEnumerable<IValue> Members

		public IEnumerator<IValue> GetEnumerator() {
			return new Enumerator(this);
		}

		#endregion


		private void ensureCapacity(int capacity) {
			if (args.Length < capacity) {
				args = new IValue[capacity + 4];
			}
		}


		private class Enumerator : IEnumerator<IValue> {

			private ArgsStorage argsStorage;
			private int index;


			public Enumerator(ArgsStorage args) {
				argsStorage = args;
				index = -1;
			}

			#region IEnumerator Members

			object IEnumerator.Current {
				get { return argsStorage[index]; }
			}

			public bool MoveNext() {
				index++;
				return index < argsStorage.ArgsCount;
			}

			public void Reset() {
				index = -1;
			}

			#endregion

			#region IEnumerator<IValue> Members

			public IValue Current {
				get { return argsStorage[index]; }
			}

			#endregion

			#region IDisposable Members

			public void Dispose() {
				argsStorage = null;
				index = -1;
			}

			#endregion
		}
	}
}
